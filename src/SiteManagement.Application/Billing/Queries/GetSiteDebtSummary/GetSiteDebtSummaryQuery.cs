using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Queries.GetSiteDebtSummary;

/// <summary>Returns the accrued / collected / outstanding totals for a site. Admin-only.</summary>
public sealed record GetSiteDebtSummaryQuery(Guid SiteId) : IQuery<SiteDebtSummaryDto>, IAdminRequest;
