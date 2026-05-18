using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Property.Queries;

namespace SiteManagement.Infrastructure.Persistence.Queries;

/// <summary>
/// EF Core-backed <see cref="ISiteQueries"/>. Every method projects
/// straight into the DTO record at the database level — no domain entity
/// materialisation, no change tracker, no eager <c>Include</c> chains
/// that could pull rows the UI doesn't need.
/// </summary>
public sealed class SiteQueries(AppDbContext dbContext) : ISiteQueries
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<IReadOnlyList<SiteListItemDto>> ListAsync(CancellationToken ct = default)
        => await _dbContext.Sites
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SiteListItemDto(
                s.Id,
                s.Name,
                s.Address,
                s.Blocks.Count,
                s.Blocks.Sum(b => b.Apartments.Count)))
            .ToListAsync(ct);

    /// <inheritdoc />
    public Task<SiteDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.Sites
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SiteDetailsDto(
                s.Id,
                s.Name,
                s.Address,
                s.Blocks
                    .OrderBy(b => b.Name)
                    .Select(b => new BlockSummaryDto(
                        b.Id,
                        b.Name.Value,
                        b.Apartments
                            .OrderBy(a => a.Number)
                            .Select(a => new ApartmentSummaryDto(
                                a.Id,
                                a.Number.Value,
                                a.Floor.Value,
                                a.Type.ToString(),
                                a.Status.ToString()))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
}
