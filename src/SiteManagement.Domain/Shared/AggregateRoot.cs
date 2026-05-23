namespace SiteManagement.Domain.Shared;

/// <summary>
/// Base class for aggregate roots: the single entity inside each consistency
/// boundary that the outside world is allowed to load, mutate, and save.
/// Domain events are raised here and not on inner entities, because only the
/// root is persisted as a unit.
/// </summary>
/// <typeparam name="TId">Identifier type (currently <see cref="Guid"/>).</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAuditableEntity
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Events raised since the last call to <see cref="ClearDomainEvents"/>.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <inheritdoc />
    public DateTime CreatedAtUtc { get; private set; }

    /// <inheritdoc />
    public Guid? CreatedBy { get; private set; }

    /// <inheritdoc />
    public DateTime? ModifiedAtUtc { get; private set; }

    /// <inheritdoc />
    public Guid? ModifiedBy { get; private set; }

    /// <inheritdoc />
    protected AggregateRoot() { }

    /// <inheritdoc />
    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>Records an event for later dispatch by the infrastructure layer.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>Drops the recorded events; the infrastructure calls this once they are dispatched.</summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
