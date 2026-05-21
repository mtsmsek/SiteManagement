namespace SiteManagement.Application.Billing.Queries;

/// <summary>One dues period row on the site's billing list.</summary>
public sealed record DuesPeriodListItemDto(
    Guid Id,
    string Month,
    decimal PerApartmentAmount,
    int ItemCount,
    int PaidCount,
    bool IsClosed);

/// <summary>One utility bill period row on the site's billing list.</summary>
public sealed record UtilityBillPeriodListItemDto(
    Guid Id,
    string Month,
    string UtilityType,
    decimal TotalAmount,
    int ItemCount,
    int PaidCount,
    bool IsClosed);

/// <summary>A single billing line a resident owes (dues or a utility).</summary>
public sealed record ResidentBillDto(
    Guid ItemId,
    Guid PeriodId,
    string Kind,
    string Month,
    string Detail,
    decimal Amount,
    string Status);

/// <summary>Per-site accrued-vs-collected summary across all periods.</summary>
public sealed record SiteDebtSummaryDto(
    Guid SiteId,
    decimal TotalAccrued,
    decimal TotalCollected,
    decimal TotalOutstanding);
