using MediatR;

namespace SiteManagement.Application.Billing.Queries.ListUtilityBillPeriodItems;

/// <summary>Lists the distributed per-apartment items of one utility bill period. Admin-only.</summary>
public sealed record ListUtilityBillPeriodItemsQuery(Guid UtilityBillPeriodId) : IRequest<IReadOnlyList<PeriodItemDto>>;
