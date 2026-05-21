using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.DistributeDues;

/// <summary>
/// Resolves the site's active occupants (via the Tenancy read side) and adds a
/// dues item for each. The period's domain methods enforce the closed-period
/// and duplicate-apartment invariants. TransactionBehavior wraps the save.
/// </summary>
public sealed class DistributeDuesCommandHandler(
    IDuesPeriodRepository duesPeriodRepository,
    ITenancyQueries tenancyQueries,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DistributeDuesCommand>
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = duesPeriodRepository;
    private readonly ITenancyQueries _tenancyQueries = tenancyQueries;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(DistributeDuesCommand request, CancellationToken cancellationToken)
    {
        var period = await _duesPeriodRepository.GetByIdAsync(request.DuesPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(DuesPeriod), request.DuesPeriodId);

        var occupants = await _tenancyQueries.GetActiveOccupantsForSiteAsync(period.SiteId, cancellationToken);

        foreach (var occupant in occupants)
        {
            // Brand-new inner entity added through a loaded aggregate is tracked
            // as "modified" by default; force it to "added" so EF INSERTs it.
            var item = period.AddItemFor(occupant.ApartmentId, occupant.ResidentId);
            _unitOfWork.MarkAsAdded(item);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
