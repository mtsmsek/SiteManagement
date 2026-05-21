using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListUtilityBillPeriodItems;

/// <summary>Delegates straight to <see cref="IBillingQueries"/> — pure read path.</summary>
public sealed class ListUtilityBillPeriodItemsQueryHandler(IBillingQueries billingQueries)
    : IRequestHandler<ListUtilityBillPeriodItemsQuery, IReadOnlyList<PeriodItemDto>>
{
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<PeriodItemDto>> Handle(ListUtilityBillPeriodItemsQuery request, CancellationToken cancellationToken)
        => _billingQueries.ListUtilityBillPeriodItemsAsync(request.UtilityBillPeriodId, cancellationToken);
}
