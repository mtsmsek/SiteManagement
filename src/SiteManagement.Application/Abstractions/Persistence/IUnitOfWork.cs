namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Persistence-agnostic transaction boundary. Handlers call
/// <see cref="SaveChangesAsync"/> exactly once at the end of a single-write
/// command; multi-write commands that need atomic commit across more than
/// one <c>SaveChangesAsync</c> call (e.g. Resident + AppUser creation)
/// open an explicit scope via <see cref="BeginScopeAsync"/>.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists every pending change in the current scope.</summary>
    /// <returns>The number of state entries written.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Opens a new transaction scope. The handler must call
    /// <see cref="IUnitOfWorkScope.CommitAsync"/> on success;
    /// <c>DisposeAsync</c> rolls the scope back if it is still uncommitted,
    /// so the <c>await using</c> pattern is both correct and try/catch-free.
    /// </summary>
    Task<IUnitOfWorkScope> BeginScopeAsync(CancellationToken ct = default);
}

/// <summary>
/// Active transaction scope. Commit on the success path, rely on
/// <c>DisposeAsync</c> to roll back when an exception bubbles out.
/// </summary>
/// <example>
/// <code>
/// await using var scope = await _uow.BeginScopeAsync(ct);
/// // ... multiple repository operations + SaveChangesAsync calls ...
/// await scope.CommitAsync(ct);
/// </code>
/// </example>
public interface IUnitOfWorkScope : IAsyncDisposable
{
    /// <summary>Commits every change made during the scope.</summary>
    Task CommitAsync(CancellationToken ct = default);
}
