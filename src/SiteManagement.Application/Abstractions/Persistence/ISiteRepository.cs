using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="Site"/> aggregate. Inherits
/// CRUD from <see cref="IRepository{TRoot}"/>; Site-specific lookup
/// queries arrive here as future commands require them. Read-side list /
/// detail views live behind <see cref="Property.Queries.ISiteQueries"/>.
/// </summary>
public interface ISiteRepository : IRepository<Site>
{
}
