namespace SiteManagement.Application.Residency.Queries;

/// <summary>
/// Read-side port for the Residency bounded context. Returns DTOs only.
/// Concrete implementation lives in
/// <c>SiteManagement.Infrastructure.Persistence.Queries.ResidentQueries</c>.
/// </summary>
public interface IResidentQueries
{
    /// <summary>Returns one row per resident with the columns the list page renders.</summary>
    Task<IReadOnlyList<ResidentListItemDto>> ListAsync(CancellationToken ct = default);

    /// <summary>Returns the full detail projection for a single resident.</summary>
    /// <returns><c>null</c> when no resident has that id.</returns>
    Task<ResidentDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Maps the given resident ids to their email addresses in one query.
    /// Used by billing notifications to email a batch of residents without an
    /// N+1 lookup.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>> GetEmailsByIdsAsync(
        IReadOnlyCollection<Guid> residentIds, CancellationToken ct = default);
}
