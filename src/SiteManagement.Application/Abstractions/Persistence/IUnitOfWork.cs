namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Persistence-agnostic transaction boundary. Handlers call
/// <see cref="SaveChangesAsync"/> exactly once at the end of a command;
/// the Infrastructure layer's EF implementation commits the DbContext
/// change tracker in a single batch.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists every pending change to the underlying store.</summary>
    /// <returns>The number of state entries written.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
