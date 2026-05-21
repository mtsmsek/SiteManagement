using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListUtilityBillPeriods;

/// <summary>Delegates straight to <see cref="IBillingQueries"/> — pure read path.</summary>
public sealed class ListUtilityBillPeriodsQueryHandler(IBillingQueries billingQueries)
    : IRequestHandler<ListUtilityBillPeriodsQuery, IReadOnlyList<UtilityBillPeriodListItemDto>>
{
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<UtilityBillPeriodListItemDto>> Handle(ListUtilityBillPeriodsQuery request, CancellationToken cancellationToken)
        => _billingQueries.ListUtilityBillPeriodsAsync(request.SiteId, cancellationToken);
}
