namespace SiteManagement.Application.Reports.Queries;

/// <summary>
/// The resident portal landing summary: what the signed-in resident owes, any
/// credit in their favour, and how many messages await them. Composed from the
/// Billing + Messaging read sides for the current resident.
/// </summary>
public sealed record ResidentDashboardDto(
    decimal TotalOutstanding,
    decimal TotalCredit,
    int UnpaidCount,
    int UnreadMessages);

/// <summary>
/// The admin dashboard summary across the whole system: how many sites and
/// residents, the money accrued vs collected (and the resulting collection
/// rate), what's still outstanding, and the credit owed back to residents.
/// </summary>
public sealed record AdminDashboardDto(
    int SiteCount,
    int ResidentCount,
    decimal TotalAccrued,
    decimal TotalCollected,
    decimal TotalOutstanding,
    decimal TotalCredit,
    decimal CollectionRate);
