namespace SiteManagement.Application.Tenancy.Queries;

/// <summary>
/// Read-side port for the Tenancy bounded context. Returns DTOs — never
/// domain entities. Concrete implementation lives in
/// <c>SiteManagement.Infrastructure.Persistence.Queries.TenancyQueries</c>.
/// </summary>
public interface ITenancyQueries
{
    /// <summary>Returns the active occupants of the given apartments, keyed for the site detail view.</summary>
    Task<IReadOnlyList<ApartmentOccupantDto>> GetActiveOccupantsForSiteAsync(Guid siteId, CancellationToken ct = default);

    /// <summary>Returns the active occupant of a single apartment, or null when empty.</summary>
    Task<ApartmentOccupantDto?> GetActiveOccupantAsync(Guid apartmentId, CancellationToken ct = default);

    /// <summary>Returns a resident's assignment history (most recent first).</summary>
    Task<IReadOnlyList<ResidentAssignmentDto>> GetAssignmentsForResidentAsync(Guid residentId, CancellationToken ct = default);
}
