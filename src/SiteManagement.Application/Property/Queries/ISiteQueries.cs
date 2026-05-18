namespace SiteManagement.Application.Property.Queries;

/// <summary>
/// Read-side port for the Property bounded context. Returns DTOs — never
/// domain entities — so the Application layer stays free of EF/LINQ-to-EF
/// internals. Concrete implementation lives in
/// <c>SiteManagement.Infrastructure.Persistence.Queries.SiteQueries</c>.
/// </summary>
public interface ISiteQueries
{
    /// <summary>Returns one row per site with the totals shown on the list page.</summary>
    Task<IReadOnlyList<SiteListItemDto>> ListAsync(CancellationToken ct = default);

    /// <summary>Returns the full detail projection for a single site.</summary>
    /// <returns><c>null</c> when no site has that id.</returns>
    Task<SiteDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
