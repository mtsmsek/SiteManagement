using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IDuesPeriodRepository"/>. The dues items are part
/// of the aggregate, so they load eagerly with the period.
/// </summary>
public sealed class DuesPeriodRepository(AppDbContext dbContext) : IDuesPeriodRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<DuesPeriod?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.DuesPeriods
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(DuesPeriod period, CancellationToken ct = default)
    {
        await _dbContext.DuesPeriods.AddAsync(period, ct);
    }

    /// <inheritdoc />
    public void Remove(DuesPeriod period) => _dbContext.DuesPeriods.Remove(period);
}
