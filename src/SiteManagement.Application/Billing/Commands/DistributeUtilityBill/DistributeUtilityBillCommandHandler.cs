using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.DistributeUtilityBill;

/// <summary>
/// Resolves the site's active occupants (via the Tenancy read side) and splits
/// the period's total amount equally across them. The period's domain method
/// enforces the closed-period and empty-distribution invariants.
/// TransactionBehavior wraps the save.
/// </summary>
public sealed class DistributeUtilityBillCommandHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    ITenancyQueries tenancyQueries,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DistributeUtilityBillCommand>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly ITenancyQueries _tenancyQueries = tenancyQueries;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(DistributeUtilityBillCommand request, CancellationToken cancellationToken)
    {
        var period = await _utilityBillPeriodRepository.GetByIdAsync(request.UtilityBillPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(UtilityBillPeriod), request.UtilityBillPeriodId);

        var occupants = await _tenancyQueries.GetActiveOccupantsForSiteAsync(period.SiteId, cancellationToken);

        var items = period.DistributeEqually(occupants.Select(o => (o.ApartmentId, o.ResidentId)).ToList());

        // Brand-new inner entities added through a loaded aggregate are tracked
        // as "modified" by default; force them to "added" so EF INSERTs them.
        foreach (var item in items)
        {
            _unitOfWork.MarkAsAdded(item);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
