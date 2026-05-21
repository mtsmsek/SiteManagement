using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListUtilityBillPeriods;

/// <summary>Lists a site's utility bill periods (most recent month first). Admin-only.</summary>
public sealed record ListUtilityBillPeriodsQuery(Guid SiteId) : IRequest<IReadOnlyList<UtilityBillPeriodListItemDto>>;
