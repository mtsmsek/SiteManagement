using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="Site"/> aggregate. Inherits
/// CRUD from <see cref="IRepository{TRoot}"/>; the find-by-child lookups
/// let handlers receive only the leaf id (BlockId / ApartmentId) from the
/// caller and resolve the owning aggregate server-side. Read-side list /
/// detail views live behind <see cref="Property.Queries.ISiteQueries"/>.
/// </summary>
public interface ISiteRepository : IRepository<Site>
{
    /// <summary>Loads the site (with blocks + apartments) that contains the given block.</summary>
    /// <returns><c>null</c> when no site contains a block with that id.</returns>
    Task<Site?> FindContainingBlockAsync(Guid blockId, CancellationToken ct = default);

    /// <summary>Loads the site (with blocks + apartments) that contains the given apartment.</summary>
    /// <returns><c>null</c> when no site contains an apartment with that id.</returns>
    Task<Site?> FindContainingApartmentAsync(Guid apartmentId, CancellationToken ct = default);

    /// <summary>
    /// Loads the site (with children) bypassing the soft-delete query filter, so
    /// an already-archived site can be hard-purged. Returns <c>null</c> if absent.
    /// </summary>
    Task<Site?> FindIncludingArchivedAsync(Guid id, CancellationToken ct = default);
}
