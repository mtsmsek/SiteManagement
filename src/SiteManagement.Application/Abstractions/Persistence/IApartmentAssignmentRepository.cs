using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="ApartmentAssignment"/> aggregate.
/// Adds a lookup for the currently active assignment of an apartment so the
/// move-out flow can resolve which assignment to end. Read-side projections
/// live behind <see cref="Tenancy.Queries.ITenancyQueries"/>.
/// </summary>
public interface IApartmentAssignmentRepository : IRepository<ApartmentAssignment>
{
    /// <summary>Loads the active (open-ended) assignment for an apartment, if any.</summary>
    /// <returns><c>null</c> when the apartment has no active assignment.</returns>
    Task<ApartmentAssignment?> FindActiveByApartmentAsync(Guid apartmentId, CancellationToken ct = default);
}
