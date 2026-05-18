using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Property;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="ISiteRepository"/>. Eager-loads the entire
/// aggregate (Site → Blocks → Apartments) inside <see cref="GetByIdAsync"/>
/// so command handlers never operate on half-hydrated state.
/// </summary>
public sealed class SiteRepository(AppDbContext dbContext) : ISiteRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Site?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.Sites
            .Include(s => s.Blocks)
            .ThenInclude(b => b.Apartments)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(Site site, CancellationToken ct = default)
    {
        await _dbContext.Sites.AddAsync(site, ct);
    }

    /// <inheritdoc />
    public void Remove(Site site) => _dbContext.Sites.Remove(site);
}
