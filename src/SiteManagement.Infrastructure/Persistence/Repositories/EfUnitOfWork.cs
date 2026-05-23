using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Shared;
using SiteManagement.Infrastructure.Persistence.Outbox;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IUnitOfWork"/>. <see cref="SaveChangesAsync"/>
/// persists the change tracker and then flushes the domain events raised by
/// the tracked aggregates through the <see cref="IDomainEventDispatcher"/>;
/// <see cref="BeginScopeAsync"/> opens a real <see cref="IDbContextTransaction"/>
/// so multiple writes (domain aggregate + Identity, multi-aggregate flows)
/// land atomically.
/// </summary>
public sealed class EfUnitOfWork(AppDbContext dbContext, IDomainEventDispatcher eventDispatcher) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IDomainEventDispatcher _eventDispatcher = eventDispatcher;

    /// <inheritdoc />
    /// <remarks>
    /// Saves first, then dispatches. A handler may mutate another aggregate
    /// (e.g. assigning a resident marks the apartment occupied), which raises
    /// further events and requires another save — so we loop until no tracked
    /// aggregate has pending events. Within a <see cref="BeginScopeAsync"/>
    /// transaction this stays atomic; the bounded loop guards against an event
    /// cycle.
    /// </remarks>
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        const int maxDispatchRounds = 10;

        var totalWritten = await _dbContext.SaveChangesAsync(ct);

        for (var round = 0; round < maxDispatchRounds; round++)
        {
            // Drains in-transaction events for dispatch and queues integration
            // events into the outbox (a pending change) in the same pass.
            var events = DrainDomainEvents();
            if (events.Count == 0 && !_dbContext.ChangeTracker.HasChanges())
            {
                break;
            }

            if (events.Count > 0)
            {
                await _eventDispatcher.DispatchAsync(events, ct);
            }

            if (_dbContext.ChangeTracker.HasChanges())
            {
                totalWritten += await _dbContext.SaveChangesAsync(ct);
            }
        }

        return totalWritten;
    }

    /// <summary>
    /// Collects and clears the domain events from every tracked aggregate root.
    /// Cleared eagerly so a follow-up save in the same flow doesn't re-dispatch.
    /// Integration events are split off and persisted to the outbox (committed in
    /// this same transaction); only the in-transaction events are returned for
    /// immediate dispatch, so a side effect like email never rides the write.
    /// </summary>
    private IReadOnlyList<IDomainEvent> DrainDomainEvents()
    {
        var roots = _dbContext.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        var events = roots.SelectMany(a => a.DomainEvents).ToList();
        foreach (var root in roots)
        {
            root.ClearDomainEvents();
        }

        var inTransactionEvents = new List<IDomainEvent>(events.Count);
        foreach (var domainEvent in events)
        {
            if (domainEvent is IIntegrationEvent integrationEvent)
            {
                _dbContext.OutboxMessages.Add(OutboxMessage.From(integrationEvent));
            }
            else
            {
                inTransactionEvents.Add(domainEvent);
            }
        }

        return inTransactionEvents;
    }

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
