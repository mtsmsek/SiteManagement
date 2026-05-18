using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IResidentRepository"/>. Vehicles are part of
/// the aggregate so they always load eagerly; <see cref="FindByTcNoAsync"/>
/// supports the duplicate-registration guard inside the create command.
/// </summary>
public sealed class ResidentRepository(AppDbContext dbContext) : IResidentRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Resident?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.Residents
            .Include(r => r.Vehicles)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    /// <inheritdoc />
    public Task<Resident?> FindByTcNoAsync(TcNo tcNo, CancellationToken ct = default)
        => _dbContext.Residents
            .Include(r => r.Vehicles)
            .FirstOrDefaultAsync(r => r.TcNo == tcNo, ct);

    /// <inheritdoc />
    public async Task AddAsync(Resident resident, CancellationToken ct = default)
    {
        await _dbContext.Residents.AddAsync(resident, ct);
    }

    /// <inheritdoc />
    public void Remove(Resident resident) => _dbContext.Residents.Remove(resident);
}
