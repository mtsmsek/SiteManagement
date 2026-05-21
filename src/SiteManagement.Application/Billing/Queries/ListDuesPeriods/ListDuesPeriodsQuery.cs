using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListDuesPeriods;

/// <summary>Lists a site's dues periods (most recent month first). Admin-only.</summary>
public sealed record ListDuesPeriodsQuery(Guid SiteId) : IRequest<IReadOnlyList<DuesPeriodListItemDto>>;
