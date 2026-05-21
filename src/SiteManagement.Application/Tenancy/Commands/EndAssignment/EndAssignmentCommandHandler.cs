using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Application.Tenancy.Commands.EndAssignment;

/// <summary>
/// Loads the assignment, ends it, and saves inside a transaction so the
/// move-out and the apartment becoming empty (raised by the assignment's
/// <c>ResidentMovedOut</c> event, applied by a Property-side handler in a
/// follow-up save) commit atomically.
/// </summary>
public sealed class EndAssignmentCommandHandler(
    IApartmentAssignmentRepository assignmentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<EndAssignmentCommand>
{
    private readonly IApartmentAssignmentRepository _assignmentRepository = assignmentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(EndAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(ApartmentAssignment), request.AssignmentId);

        assignment.End(request.EndDate);

        // The move-out event frees the apartment in a follow-up save;
        // TransactionBehavior keeps both writes atomic.
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
