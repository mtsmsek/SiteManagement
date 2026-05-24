using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Application.Tenancy.Commands.AssignResident;

/// <summary>
/// Creates an <see cref="ApartmentAssignment"/> and saves inside a transaction
/// so the assignment INSERT and the apartment-occupancy flip — raised by the
/// assignment's <c>ResidentAssignedToApartment</c> event and applied by a
/// Property-side handler in a follow-up save — commit atomically. The apartment
/// and resident must both exist, and the apartment must be free. "Already
/// assigned" is a cross-aggregate rule (it spans other assignments), so it's
/// checked here via the read side rather than inside the aggregate; the DB's
/// filtered unique index stays as the last line of defence against a race.
/// </summary>
public sealed class AssignResidentCommandHandler(
    IApartmentAssignmentRepository assignmentRepository,
    ISiteRepository siteRepository,
    IResidentRepository residentRepository,
    ITenancyQueries tenancyQueries,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignResidentCommand, AssignResidentResult>
{
    private readonly IApartmentAssignmentRepository _assignmentRepository = assignmentRepository;
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IResidentRepository _residentRepository = residentRepository;
    private readonly ITenancyQueries _tenancyQueries = tenancyQueries;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<AssignResidentResult> Handle(AssignResidentCommand request, CancellationToken cancellationToken)
    {
        // Both referenced aggregates must exist before we link them.
        _ = await _siteRepository.FindContainingApartmentAsync(request.ApartmentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Apartment), request.ApartmentId);
        _ = await _residentRepository.GetByIdAsync(request.ResidentId, cancellationToken)
            ?? throw new EntityNotFoundException("Resident", request.ResidentId);

        // Reject up front if the apartment is already occupied, so the caller
        // gets a clean 409 instead of the DB index surfacing as a 500.
        var existingOccupant = await _tenancyQueries.GetActiveOccupantAsync(request.ApartmentId, cancellationToken);
        if (existingOccupant is not null)
        {
            throw new BusinessRuleViolationException(
                ErrorMessageKeys.TenancyApartmentAlreadyAssigned,
                ErrorMessageKeys.TenancyApartmentAlreadyAssigned);
        }

        var assignment = ApartmentAssignment.Assign(
            request.ApartmentId, request.ResidentId, request.TenantType, request.StartDate);

        await _assignmentRepository.AddAsync(assignment, cancellationToken);

        // The assignment's event makes a Property-side handler mark the
        // apartment occupied (a second save in the same flow). TransactionBehavior
        // wraps every ICommand, so both writes commit atomically without this
        // handler managing a scope.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AssignResidentResult(assignment.Id);
    }
}
