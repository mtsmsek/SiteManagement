namespace PaymentService.Domain.Shared;

/// <summary>
/// Base class for aggregate roots: the single entity inside each consistency
/// boundary that the outside world loads, mutates, and saves as a unit. A
/// deliberate copy of the main API's primitive — PaymentService shares no code
/// with the main API by design, only HTTP contracts, so its release cycle stays
/// independent. No domain-event list here: this service is invoked synchronously
/// and raises no cross-aggregate events.
/// </summary>
/// <typeparam name="TId">Identifier type (currently <see cref="Guid"/>).</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    /// <inheritdoc />
    protected AggregateRoot() { }

    /// <inheritdoc />
    protected AggregateRoot(TId id) : base(id) { }
}
