using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Reports.Queries;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Infrastructure.Persistence.Queries;

/// <summary>
/// EF Core-backed <see cref="IReportQueries"/>. Aggregates across the whole
/// system for the admin dashboard. Sites honour the global soft-delete filter,
/// so archived sites are excluded from the count. No change tracking.
/// </summary>
public sealed class ReportQueries(AppDbContext dbContext) : IReportQueries
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken ct = default)
    {
        var siteCount = await _dbContext.Sites.AsNoTracking().CountAsync(ct);
        var residentCount = await _dbContext.Residents.AsNoTracking().CountAsync(ct);

        var duesItems = await _dbContext.DuesPeriods
            .AsNoTracking()
            .SelectMany(p => p.Items.Select(i => new { i.Amount.Amount, i.Status }))
            .ToListAsync(ct);

        var utilityItems = await _dbContext.UtilityBillPeriods
            .AsNoTracking()
            .SelectMany(p => p.Items.Select(i => new { i.Amount.Amount, i.Status }))
            .ToListAsync(ct);

        var all = duesItems.Concat(utilityItems).ToList();
        var accrued = all.Sum(i => i.Amount);
        var collected = all.Where(i => i.Status == BillingItemStatus.Paid).Sum(i => i.Amount);

        var balances = await _dbContext.ResidentCreditAccounts
            .AsNoTracking()
            .Select(a => a.Balance.Amount)
            .ToListAsync(ct);
        var credit = balances.Sum();

        var collectionRate = accrued > 0 ? collected / accrued : 0m;

        return new AdminDashboardDto(
            siteCount,
            residentCount,
            accrued,
            collected,
            accrued - collected,
            credit,
            collectionRate);
    }
}
