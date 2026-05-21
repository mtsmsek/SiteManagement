using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IApartmentAssignmentRepository"/>. The aggregate
/// has no inner collections, so loads are simple; the active-assignment lookup
/// filters on the open period (no end date).
/// </summary>
public sealed class ApartmentAssignmentRepository(AppDbContext dbContext) : IApartmentAssignmentRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<ApartmentAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.ApartmentAssignments.FirstOrDefaultAsync(a => a.Id == id, ct);

    /// <inheritdoc />
    public Task<ApartmentAssignment?> FindActiveByApartmentAsync(Guid apartmentId, CancellationToken ct = default)
        => _dbContext.ApartmentAssignments
            .FirstOrDefaultAsync(a => a.ApartmentId == apartmentId && a.Period.EndDate == null, ct);

    /// <inheritdoc />
    public async Task AddAsync(ApartmentAssignment assignment, CancellationToken ct = default)
    {
        await _dbContext.ApartmentAssignments.AddAsync(assignment, ct);
    }

    /// <inheritdoc />
    public void Remove(ApartmentAssignment assignment) => _dbContext.ApartmentAssignments.Remove(assignment);
}
