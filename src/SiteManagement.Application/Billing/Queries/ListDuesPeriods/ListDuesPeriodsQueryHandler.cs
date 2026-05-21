using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListDuesPeriods;

/// <summary>Delegates straight to <see cref="IBillingQueries"/> — pure read path.</summary>
public sealed class ListDuesPeriodsQueryHandler(IBillingQueries billingQueries)
    : IRequestHandler<ListDuesPeriodsQuery, IReadOnlyList<DuesPeriodListItemDto>>
{
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<DuesPeriodListItemDto>> Handle(ListDuesPeriodsQuery request, CancellationToken cancellationToken)
        => _billingQueries.ListDuesPeriodsAsync(request.SiteId, cancellationToken);
}
