namespace SiteManagement.Domain.Shared;

/// <summary>
/// Base class for domain entities. An entity is anything that is
/// distinguished by an <see cref="Id"/> rather than by the values of its
/// fields: two entities with the same id are the same entity even if their
/// state differs.
/// </summary>
/// <typeparam name="TId">The identifier type (currently <see cref="Guid"/> across the project).</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>Stable identifier; assigned at creation time.</summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>Required for EF Core materialisation; never called from production code.</summary>
    protected Entity() { }

    /// <summary>Creates the entity with the given identifier.</summary>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <inheritdoc />
    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
        {
            return false;
        }

        // Transient entities (default Id) are only equal to themselves.
        if (EqualityComparer<TId>.Default.Equals(Id, default!)
            || EqualityComparer<TId>.Default.Equals(other.Id, default!))
        {
            return ReferenceEquals(this, other);
        }

        return GetType() == other.GetType()
            && EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Entity<TId> e && Equals(e);

    /// <inheritdoc />
    public override int GetHashCode()
        => EqualityComparer<TId>.Default.GetHashCode(Id);

    /// <summary>Identity-based equality operator.</summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>Identity-based inequality operator.</summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}
