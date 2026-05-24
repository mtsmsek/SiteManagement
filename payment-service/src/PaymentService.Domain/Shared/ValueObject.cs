namespace PaymentService.Domain.Shared;

/// <summary>
/// Base class for value objects: equality is computed from the components
/// returned by <see cref="GetEqualityComponents"/>. A deliberate copy of the
/// main API's primitive — see <see cref="AggregateRoot{TId}"/> for why the
/// services don't share code.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>Components that participate in equality &amp; hashing.</summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc />
    public bool Equals(ValueObject? other)
    {
        if (other is null)
        {
            return false;
        }

        return GetType() == other.GetType()
            && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ValueObject vo && Equals(vo);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }
        return hash.ToHashCode();
    }

    /// <summary>Value-equality operator: forwards to <see cref="Equals(ValueObject)"/>.</summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>Value-inequality operator.</summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
