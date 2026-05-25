using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.DistributeUtilityBill;

/// <summary>
/// Resolves the site's active occupants (via the Tenancy read side) and splits
/// the period's total amount equally across them. The period's domain method
/// enforces the closed-period and empty-distribution invariants. A resident
/// holding enough credit from a prior over-payment has their new share settled
/// from that credit straight away. TransactionBehavior wraps the save.
/// </summary>
public sealed class DistributeUtilityBillCommandHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    ITenancyQueries tenancyQueries,
    IResidentCreditService creditService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DistributeUtilityBillCommand>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly ITenancyQueries _tenancyQueries = tenancyQueries;
    private readonly IResidentCreditService _creditService = creditService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(DistributeUtilityBillCommand request, CancellationToken cancellationToken)
    {
        var period = await _utilityBillPeriodRepository.GetByIdAsync(request.UtilityBillPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(UtilityBillPeriod), request.UtilityBillPeriodId);

        var occupants = await _tenancyQueries.GetActiveOccupantsForSiteAsync(period.SiteId, cancellationToken);

        var items = period.DistributeEqually(occupants.Select(o => (o.ApartmentId, o.ResidentId)).ToList());

        foreach (var item in items)
        {
            // Brand-new inner entities added through a loaded aggregate are tracked
            // as "modified" by default; force them to "added" so EF INSERTs them.
            _unitOfWork.MarkAsAdded(item);

            // Settle the fresh share from any credit the resident is owed.
            if (await _creditService.TryConsumeAsync(item.ResidentId, item.Amount, cancellationToken))
            {
                period.MarkItemPaid(item.Id);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
