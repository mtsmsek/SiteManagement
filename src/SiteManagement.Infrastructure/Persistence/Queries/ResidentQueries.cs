using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Residency.Queries;

namespace SiteManagement.Infrastructure.Persistence.Queries;

/// <summary>
/// EF Core-backed <see cref="IResidentQueries"/>. Every method projects
/// straight into the DTO record at the database level so the Application
/// layer never sees a domain entity for read paths.
/// </summary>
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
    public Task<ResidentDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.Residents
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new ResidentDetailsDto(
                r.Id,
                r.TcNo.Value,
                r.FullName.FirstName,
                r.FullName.LastName,
                r.Email.Value,
                r.Phone.Value,
                r.Vehicles
                    .OrderBy(v => v.Plate.Value)
                    .Select(v => new VehicleDto(v.Plate.Value, v.Note))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
}
