using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Property;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="ISiteRepository"/>. Every loader eager-loads
/// the full aggregate (Site → Blocks → Apartments) so command handlers
/// always operate on fully-hydrated state. The two find-by-child methods
/// stay here because they only follow shadow FKs inside the same aggregate
/// — pure persistence concern, no business state evaluated.
/// </summary>
public sealed class SiteRepository(AppDbContext dbContext) : ISiteRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Site?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => HydratedSites().FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <inheritdoc />
    public Task<Site?> FindContainingBlockAsync(Guid blockId, CancellationToken ct = default)
        => HydratedSites().FirstOrDefaultAsync(s => s.Blocks.Any(b => b.Id == blockId), ct);

    /// <inheritdoc />
    public Task<Site?> FindContainingApartmentAsync(Guid apartmentId, CancellationToken ct = default)
        => HydratedSites().FirstOrDefaultAsync(
            s => s.Blocks.Any(b => b.Apartments.Any(a => a.Id == apartmentId)), ct);

    /// <inheritdoc />
    public Task<Site?> FindIncludingArchivedAsync(Guid id, CancellationToken ct = default)
        => HydratedSites().IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(Site site, CancellationToken ct = default)
    {
        await _dbContext.Sites.AddAsync(site, ct);
    }

    /// <inheritdoc />
    public void Remove(Site site) => _dbContext.Sites.Remove(site);

    /// <summary>Single query shape reused by every loader so command-side state is consistent.</summary>
    private IQueryable<Site> HydratedSites()
        => _dbContext.Sites
            .Include(s => s.Blocks)
            .ThenInclude(b => b.Apartments);
}
