using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Queries.ListDuesPeriods;

/// <summary>Lists a site's dues periods (most recent month first). Admin-only.</summary>
public sealed record ListDuesPeriodsQuery(Guid SiteId) : IQuery<IReadOnlyList<DuesPeriodListItemDto>>;
