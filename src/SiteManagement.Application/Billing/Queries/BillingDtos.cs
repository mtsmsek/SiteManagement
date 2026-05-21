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

/// <summary>
/// One distributed line within a period (dues or utility), enriched with the
/// apartment label and occupant name the admin item table shows. Dues and
/// utility items share this shape, so a single record serves both.
/// </summary>
public sealed record PeriodItemDto(
    Guid ItemId,
    Guid ApartmentId,
    string Apartment,
    Guid ResidentId,
    string ResidentFullName,
    decimal Amount,
    string Status);

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
