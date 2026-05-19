using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SiteManagement.Application.Abstractions.Persistence;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IUnitOfWork"/>. Delegates
/// <see cref="SaveChangesAsync"/> straight to the
/// <see cref="AppDbContext"/> change tracker; <see cref="BeginScopeAsync"/>
/// opens a real <see cref="IDbContextTransaction"/> so multiple writes
/// (domain aggregate + Identity, multi-aggregate flows) land atomically.
/// </summary>
public sealed class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);

    /// <inheritdoc />
    public void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class
        => _dbContext.Entry(entity).State = EntityState.Added;

    /// <inheritdoc />
    public async Task<IUnitOfWorkScope> BeginScopeAsync(CancellationToken ct = default)
    {
        // Re-entrancy: an outer scope (e.g. an integration test ambient
        // transaction) already owns the connection. Hand back a no-op scope
        // so handler code is identical in both contexts.
        if (_dbContext.Database.CurrentTransaction is not null)
        {
            return new NoOpScope();
        }

        var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        return new EfUnitOfWorkScope(transaction);
    }

    /// <summary>
    /// Wraps a real EF transaction. Commit is explicit; DisposeAsync rolls
    /// back when commit was never reached, giving callers a try/catch-free
    /// success path.
    /// </summary>
    private sealed class EfUnitOfWorkScope(IDbContextTransaction transaction) : IUnitOfWorkScope
    {
        private readonly IDbContextTransaction _transaction = transaction;
        private bool _committed;

        public async Task CommitAsync(CancellationToken ct = default)
        {
            await _transaction.CommitAsync(ct);
            _committed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_committed)
            {
                await _transaction.RollbackAsync();
            }

            await _transaction.DisposeAsync();
        }
    }

    /// <summary>No-op scope used when a real outer transaction already exists.</summary>
    private sealed class NoOpScope : IUnitOfWorkScope
    {
        public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
