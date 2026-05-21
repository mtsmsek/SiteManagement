using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListResidentBills;

/// <summary>Delegates straight to <see cref="IBillingQueries"/> — pure read path.</summary>
public sealed class ListResidentBillsQueryHandler(IBillingQueries billingQueries)
    : IRequestHandler<ListResidentBillsQuery, IReadOnlyList<ResidentBillDto>>
{
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<ResidentBillDto>> Handle(ListResidentBillsQuery request, CancellationToken cancellationToken)
        => _billingQueries.ListResidentBillsAsync(request.ResidentId, cancellationToken);
}
