using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="Resident"/> aggregate. Adds
/// the citizenship-number lookup on top of the generic CRUD contract so
/// register / update commands can detect duplicates before saving.
/// Read-side list / detail views live behind
/// <see cref="Residency.Queries.IResidentQueries"/>.
/// </summary>
public interface IResidentRepository : IRepository<Resident>
{
    /// <summary>Locates the resident registered under the given citizenship number.</summary>
    /// <returns><c>null</c> when no resident is registered with that TcNo.</returns>
    Task<Resident?> FindByTcNoAsync(TcNo tcNo, CancellationToken ct = default);
}
