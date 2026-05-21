using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Residency.Queries;

namespace SiteManagement.Infrastructure.Persistence.Queries;

/// <summary>
/// EF Core-backed <see cref="IResidentQueries"/>. Projections target the
/// DTO records at the database level so the Application layer never sees
/// a domain entity on the read path.
/// </summary>
/// <remarks>
/// <see cref="GetByIdAsync"/> hydrates the vehicles client-side rather
/// than inlining <c>Vehicles.Select(...)</c> in the SQL projection: EF Core
/// 10 cannot translate ordered + projected owned collections (see
/// "ShapedQueryExpression … cannot be translated"). Splitting the load
/// keeps the query in pure-SQL territory.
/// </remarks>
public sealed class ResidentQueries(AppDbContext dbContext) : IResidentQueries
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResidentListItemDto>> ListAsync(CancellationToken ct = default)
        => await _dbContext.Residents
            .AsNoTracking()
            .OrderBy(r => r.FullName.LastName)
            .ThenBy(r => r.FullName.FirstName)
            .Select(r => new ResidentListItemDto(
                r.Id,
                r.TcNo.Value,
                r.FullName.FirstName,
                r.FullName.LastName,
                r.Email.Value,
                r.Phone.Value,
                r.Vehicles.Count))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<ResidentDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var resident = await _dbContext.Residents
            .AsNoTracking()
            .Include(r => r.Vehicles)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (resident is null)
        {
            return null;
        }

        var vehicles = resident.Vehicles
            .OrderBy(v => v.Plate.Value)
            .Select(v => new VehicleDto(v.Plate.Value, v.Note))
            .ToList();

        return new ResidentDetailsDto(
            resident.Id,
            resident.TcNo.Value,
            resident.FullName.FirstName,
            resident.FullName.LastName,
            resident.Email.Value,
            resident.Phone.Value,
            vehicles);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<Guid, string>> GetEmailsByIdsAsync(
        IReadOnlyCollection<Guid> residentIds, CancellationToken ct = default)
    {
        if (residentIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await _dbContext.Residents
            .AsNoTracking()
            .Where(r => residentIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Email.Value, ct);
    }
}
