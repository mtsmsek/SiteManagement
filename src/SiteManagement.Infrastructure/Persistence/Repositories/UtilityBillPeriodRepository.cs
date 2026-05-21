using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IUtilityBillPeriodRepository"/>. The bill items
/// are part of the aggregate, so they load eagerly with the period.
/// </summary>
public sealed class UtilityBillPeriodRepository(AppDbContext dbContext) : IUtilityBillPeriodRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<UtilityBillPeriod?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.UtilityBillPeriods
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(UtilityBillPeriod period, CancellationToken ct = default)
    {
        await _dbContext.UtilityBillPeriods.AddAsync(period, ct);
    }

    /// <inheritdoc />
    public void Remove(UtilityBillPeriod period) => _dbContext.UtilityBillPeriods.Remove(period);
}
