using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListDuesPeriodItems;

/// <summary>Lists the distributed per-apartment items of one dues period. Admin-only.</summary>
public sealed record ListDuesPeriodItemsQuery(Guid DuesPeriodId) : IRequest<IReadOnlyList<PeriodItemDto>>;
