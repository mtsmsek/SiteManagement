using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Queries.ListDuesPeriodItems;

/// <summary>Lists the distributed per-apartment items of one dues period. Admin-only.</summary>
public sealed record ListDuesPeriodItemsQuery(Guid DuesPeriodId) : IQuery<IReadOnlyList<PeriodItemDto>>, IAdminRequest;
