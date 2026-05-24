namespace PaymentService.Domain.Shared;

/// <summary>
/// Base class for domain entities — objects with a stable identity that
/// persists across state changes. Equality is by id, not by attribute values.
/// A deliberate copy of the main API's primitive: PaymentService is an
/// independent service and shares no code with it, only HTTP contracts.
/// </summary>
/// <typeparam name="TId">Identifier type (currently <see cref="Guid"/>).</typeparam>
public abstract class Entity<TId>
    where TId : notnull
{
    /// <summary>The entity's stable identity.</summary>
    public TId Id { get; protected set; } = default!;

    /// <inheritdoc />
    protected Entity() { }

    /// <inheritdoc />
    protected Entity(TId id) => Id = id;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Entity<TId> other && GetType() == other.GetType() && Id.Equals(other.Id);

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();
}
