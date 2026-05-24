using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Queries.ListUtilityBillPeriods;

/// <summary>Lists a site's utility bill periods (most recent month first). Admin-only.</summary>
public sealed record ListUtilityBillPeriodsQuery(Guid SiteId) : IQuery<IReadOnlyList<UtilityBillPeriodListItemDto>>;
