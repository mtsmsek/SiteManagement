using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListDuesPeriodItems;

/// <summary>Delegates straight to <see cref="IBillingQueries"/> — pure read path.</summary>
public sealed class ListDuesPeriodItemsQueryHandler(IBillingQueries billingQueries)
    : IRequestHandler<ListDuesPeriodItemsQuery, IReadOnlyList<PeriodItemDto>>
{
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<PeriodItemDto>> Handle(ListDuesPeriodItemsQuery request, CancellationToken cancellationToken)
        => _billingQueries.ListDuesPeriodItemsAsync(request.DuesPeriodId, cancellationToken);
}
