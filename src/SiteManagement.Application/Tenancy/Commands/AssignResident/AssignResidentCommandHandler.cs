using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Application.Tenancy.Commands.AssignResident;

/// <summary>
/// Creates an <see cref="ApartmentAssignment"/> and saves inside a transaction
/// so the assignment INSERT and the apartment-occupancy flip — raised by the
/// assignment's <c>ResidentAssignedToApartment</c> event and applied by a
/// Property-side handler in a follow-up save — commit atomically. The
/// apartment and resident must both exist; the DB's filtered unique index is
/// the final guard against a second active assignment on the same apartment.
/// </summary>
public sealed class AssignResidentCommandHandler(
    IApartmentAssignmentRepository assignmentRepository,
    ISiteRepository siteRepository,
    IResidentRepository residentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignResidentCommand, AssignResidentResult>
{
    private readonly IApartmentAssignmentRepository _assignmentRepository = assignmentRepository;
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IResidentRepository _residentRepository = residentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<AssignResidentResult> Handle(AssignResidentCommand request, CancellationToken cancellationToken)
    {
        // Both referenced aggregates must exist before we link them.
        _ = await _siteRepository.FindContainingApartmentAsync(request.ApartmentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Apartment), request.ApartmentId);
        _ = await _residentRepository.GetByIdAsync(request.ResidentId, cancellationToken)
            ?? throw new EntityNotFoundException("Resident", request.ResidentId);

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
