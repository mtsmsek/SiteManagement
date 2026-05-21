namespace SiteManagement.Application.Billing.Queries;

/// <summary>
/// Read-side port for the Billing bounded context. Returns DTOs — never
/// domain entities. Concrete implementation lives in
/// <c>SiteManagement.Infrastructure.Persistence.Queries.BillingQueries</c>.
/// </summary>
public interface IBillingQueries
{
    /// <summary>Lists a site's dues periods (most recent month first).</summary>
    Task<IReadOnlyList<DuesPeriodListItemDto>> ListDuesPeriodsAsync(Guid siteId, CancellationToken ct = default);

    /// <summary>Lists a site's utility bill periods (most recent month first).</summary>
    Task<IReadOnlyList<UtilityBillPeriodListItemDto>> ListUtilityBillPeriodsAsync(Guid siteId, CancellationToken ct = default);

    /// <summary>Lists the distributed per-apartment items of one dues period.</summary>
    Task<IReadOnlyList<PeriodItemDto>> ListDuesPeriodItemsAsync(Guid duesPeriodId, CancellationToken ct = default);

    /// <summary>Lists the distributed per-apartment items of one utility bill period.</summary>
    Task<IReadOnlyList<PeriodItemDto>> ListUtilityBillPeriodItemsAsync(Guid utilityBillPeriodId, CancellationToken ct = default);

    /// <summary>Returns every outstanding + paid line owed by a resident.</summary>
    Task<IReadOnlyList<ResidentBillDto>> ListResidentBillsAsync(Guid residentId, CancellationToken ct = default);

    /// <summary>Returns the accrued / collected / outstanding totals for a site.</summary>
    Task<SiteDebtSummaryDto> GetSiteDebtSummaryAsync(Guid siteId, CancellationToken ct = default);
}
