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

    /// <summary>
    /// Marks a child entity as inserted in the current change-tracker.
    /// Workaround for EF Core's behaviour when a brand-new inner entity is
    /// added to a tracked aggregate via a domain method (e.g.
    /// <c>site.AddBlock(...)</c>): EF treats the new child as <em>modified</em>
    /// and tries to <c>UPDATE</c> a row that doesn't exist yet. Calling
    /// this right after the domain mutation flips the entry state back to
    /// <em>added</em> so the INSERT goes out instead.
    /// </summary>
    void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class;
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
