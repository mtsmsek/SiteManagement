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
    public Task<IReadOnlyList<PeriodItemDto>> ListDuesPeriodItemsAsync(Guid duesPeriodId, CancellationToken ct = default)
        => ProjectPeriodItemsAsync(
            _dbContext.DuesPeriods
                .AsNoTracking()
                .Where(p => p.Id == duesPeriodId)
                .SelectMany(p => p.Items)
                .Select(i => new RawPeriodItem(i.Id, i.ApartmentId, i.ResidentId, i.Amount.Amount, i.Status.ToString())),
            ct);

    /// <inheritdoc />
    public Task<IReadOnlyList<PeriodItemDto>> ListUtilityBillPeriodItemsAsync(Guid utilityBillPeriodId, CancellationToken ct = default)
        => ProjectPeriodItemsAsync(
            _dbContext.UtilityBillPeriods
                .AsNoTracking()
                .Where(p => p.Id == utilityBillPeriodId)
                .SelectMany(p => p.Items)
                .Select(i => new RawPeriodItem(i.Id, i.ApartmentId, i.ResidentId, i.Amount.Amount, i.Status.ToString())),
            ct);

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

    /// <summary>
    /// Enriches a period's raw items with the apartment label ("A-1") and the
    /// occupant's name. Owned-collection items don't join cleanly to the other
    /// read sides in one query, so the raw lines are materialised first and the
    /// Property/Residency lookups are resolved separately, then stitched and
    /// formatted in memory.
    /// </summary>
    private async Task<IReadOnlyList<PeriodItemDto>> ProjectPeriodItemsAsync(
        IQueryable<RawPeriodItem> items, CancellationToken ct)
    {
        var rawItems = await items.ToListAsync(ct);
        if (rawItems.Count == 0)
        {
            return [];
        }

        var apartmentIds = rawItems.Select(i => i.ApartmentId).Distinct().ToList();
        var residentIds = rawItems.Select(i => i.ResidentId).Distinct().ToList();

        var apartmentLabels = (await (
                from s in _dbContext.Sites.AsNoTracking()
                from b in s.Blocks
                from a in b.Apartments
                where apartmentIds.Contains(a.Id)
                select new { a.Id, BlockName = b.Name.Value, Number = a.Number.Value })
                .ToListAsync(ct))
            .ToDictionary(x => x.Id, x => $"{x.BlockName}-{x.Number}");

        var residentNames = (await _dbContext.Residents
                .AsNoTracking()
                .Where(r => residentIds.Contains(r.Id))
                .Select(r => new { r.Id, r.FullName.FirstName, r.FullName.LastName })
                .ToListAsync(ct))
            .ToDictionary(x => x.Id, x => $"{x.FirstName} {x.LastName}");

        return rawItems
            .Select(i => new PeriodItemDto(
                i.ItemId,
                i.ApartmentId,
                apartmentLabels.GetValueOrDefault(i.ApartmentId, string.Empty),
                i.ResidentId,
                residentNames.GetValueOrDefault(i.ResidentId, string.Empty),
                i.Amount,
                i.Status))
            .ToList();
    }

    /// <summary>The period-agnostic shape both dues and utility items project to before enrichment.</summary>
    private sealed record RawPeriodItem(Guid ItemId, Guid ApartmentId, Guid ResidentId, decimal Amount, string Status);
}
