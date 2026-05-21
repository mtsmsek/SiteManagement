using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Infrastructure.Persistence.Queries;

/// <summary>
/// EF Core-backed <see cref="ITenancyQueries"/>. Projects assignments joined to
/// the resident's name straight into DTOs — no domain materialisation, no
/// change tracking. The resident name join is a read-side concern only; the
/// command side still references residents by id.
/// </summary>
public sealed class TenancyQueries(AppDbContext dbContext) : ITenancyQueries
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<IReadOnlyList<ApartmentOccupantDto>> GetActiveOccupantsForSiteAsync(
        Guid siteId, CancellationToken ct = default)
    {
        // Apartment ids belonging to the site (through its blocks).
        var apartmentIds = await _dbContext.Sites
            .AsNoTracking()
            .Where(s => s.Id == siteId)
            .SelectMany(s => s.Blocks.SelectMany(b => b.Apartments.Select(a => a.Id)))
            .ToListAsync(ct);

        return await ActiveOccupantQuery(a => apartmentIds.Contains(a.ApartmentId))
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public Task<ApartmentOccupantDto?> GetActiveOccupantAsync(Guid apartmentId, CancellationToken ct = default)
        => ActiveOccupantQuery(a => a.ApartmentId == apartmentId)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResidentAssignmentDto>> GetAssignmentsForResidentAsync(
        Guid residentId, CancellationToken ct = default)
        => await _dbContext.ApartmentAssignments
            .AsNoTracking()
            .Where(a => a.ResidentId == residentId)
            .OrderByDescending(a => a.Period.StartDate)
            .Select(a => new ResidentAssignmentDto(
                a.Id,
                a.ApartmentId,
                a.TenantType.ToString(),
                a.Period.StartDate,
                a.Period.EndDate,
                a.Period.EndDate == null))
            .ToListAsync(ct);

    /// <summary>
    /// Active assignments matching <paramref name="apartmentFilter"/>, joined to the
    /// resident's name and projected to the occupant DTO. The filter is applied to
    /// the assignment entity <em>before</em> projection so it stays translatable —
    /// filtering the projected DTO (with its joined name concatenation) does not.
    /// </summary>
    private IQueryable<ApartmentOccupantDto> ActiveOccupantQuery(
        Expression<Func<ApartmentAssignment, bool>> apartmentFilter)
        => from a in _dbContext.ApartmentAssignments.AsNoTracking().Where(apartmentFilter)
           where a.Period.EndDate == null
           join r in _dbContext.Residents.AsNoTracking() on a.ResidentId equals r.Id
           select new ApartmentOccupantDto(
               a.Id,
               a.ApartmentId,
               a.ResidentId,
               r.FullName.FirstName + " " + r.FullName.LastName,
               a.TenantType.ToString(),
               a.Period.StartDate);
}
