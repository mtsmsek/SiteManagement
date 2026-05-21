using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Infrastructure.Persistence.Queries;

/// <summary>
/// EF Core-backed <see cref="IBillingQueries"/>. Projects dues + utility
/// periods and their items straight into DTOs. A resident's bills union the
/// two item kinds; the site debt summary aggregates accrued vs collected
/// across both. No domain materialisation, no change tracking.
/// </summary>
public sealed class BillingQueries(AppDbContext dbContext) : IBillingQueries
{
    private const string DuesKind = "Dues";
    private const string UtilityKind = "Utility";

    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<IReadOnlyList<DuesPeriodListItemDto>> ListDuesPeriodsAsync(Guid siteId, CancellationToken ct = default)
        => await _dbContext.DuesPeriods
            .AsNoTracking()
            .Where(p => p.SiteId == siteId)
            .OrderByDescending(p => p.Month)
            .Select(p => new DuesPeriodListItemDto(
                p.Id,
                p.Month.ToString(),
                p.PerApartmentAmount.Amount,
                p.Items.Count,
                p.Items.Count(i => i.Status == BillingItemStatus.Paid),
                p.IsClosed))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<UtilityBillPeriodListItemDto>> ListUtilityBillPeriodsAsync(Guid siteId, CancellationToken ct = default)
        => await _dbContext.UtilityBillPeriods
            .AsNoTracking()
            .Where(p => p.SiteId == siteId)
            .OrderByDescending(p => p.Month)
            .Select(p => new UtilityBillPeriodListItemDto(
                p.Id,
                p.Month.ToString(),
                p.UtilityType.ToString(),
                p.TotalAmount.Amount,
                p.Items.Count,
                p.Items.Count(i => i.Status == BillingItemStatus.Paid),
                p.IsClosed))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResidentBillDto>> ListResidentBillsAsync(Guid residentId, CancellationToken ct = default)
    {
        var dues = await _dbContext.DuesPeriods
            .AsNoTracking()
            .SelectMany(p => p.Items
                .Where(i => i.ResidentId == residentId)
                .Select(i => new ResidentBillDto(
                    i.Id, p.Id, DuesKind, p.Month.ToString(), DuesKind, i.Amount.Amount, i.Status.ToString())))
            .ToListAsync(ct);

        var utilities = await _dbContext.UtilityBillPeriods
            .AsNoTracking()
            .SelectMany(p => p.Items
                .Where(i => i.ResidentId == residentId)
                .Select(i => new ResidentBillDto(
                    i.Id, p.Id, UtilityKind, p.Month.ToString(), p.UtilityType.ToString(), i.Amount.Amount, i.Status.ToString())))
            .ToListAsync(ct);

        return dues.Concat(utilities)
            .OrderByDescending(b => b.Month)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<SiteDebtSummaryDto> GetSiteDebtSummaryAsync(Guid siteId, CancellationToken ct = default)
    {
        var duesItems = await _dbContext.DuesPeriods
            .AsNoTracking()
            .Where(p => p.SiteId == siteId)
            .SelectMany(p => p.Items.Select(i => new { i.Amount.Amount, i.Status }))
            .ToListAsync(ct);

        var utilityItems = await _dbContext.UtilityBillPeriods
            .AsNoTracking()
            .Where(p => p.SiteId == siteId)
            .SelectMany(p => p.Items.Select(i => new { i.Amount.Amount, i.Status }))
            .ToListAsync(ct);

        var all = duesItems.Concat(utilityItems).ToList();
        var accrued = all.Sum(i => i.Amount);
        var collected = all.Where(i => i.Status == BillingItemStatus.Paid).Sum(i => i.Amount);

        return new SiteDebtSummaryDto(siteId, accrued, collected, accrued - collected);
    }
}
