using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Property.ValueObjects;

/// <summary>
/// Signed floor number. Zero is the ground floor; negative values mean
/// basement. Range is bounded by <see cref="PropertyLimits.FloorMin"/> and
/// <see cref="PropertyLimits.FloorMax"/>.
/// </summary>
public sealed class Floor : ValueObject
{
    /// <summary>Underlying signed value.</summary>
    public int Value { get; }

    /// <summary>True when the floor is a basement (negative).</summary>
    public bool IsBasement => Value < 0;

    private Floor(int value)
    {
        Value = value;
    }

    /// <summary>Creates a floor, throwing when out of range.</summary>
    /// <exception cref="FloorOutOfRangeException">Thrown for any value outside the allowed range.</exception>
    public static Floor From(int value)
    {
        if (value < PropertyLimits.FloorMin || value > PropertyLimits.FloorMax)
        {
            throw new FloorOutOfRangeException(value);
        }

        return new Floor(value);
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
