using MediatR;

namespace SiteManagement.Application.Billing.Queries.GetSiteDebtSummary;

/// <summary>Delegates straight to <see cref="IBillingQueries"/> — pure read path.</summary>
public sealed class GetSiteDebtSummaryQueryHandler(IBillingQueries billingQueries)
    : IRequestHandler<GetSiteDebtSummaryQuery, SiteDebtSummaryDto>
{
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public Task<SiteDebtSummaryDto> Handle(GetSiteDebtSummaryQuery request, CancellationToken cancellationToken)
        => _billingQueries.GetSiteDebtSummaryAsync(request.SiteId, cancellationToken);
}
