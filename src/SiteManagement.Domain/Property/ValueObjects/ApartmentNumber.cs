using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Property.ValueObjects;

/// <summary>
/// Apartment number within a block. Constrained to the inclusive range
/// [<see cref="PropertyLimits.ApartmentNumberMin"/>..
/// <see cref="PropertyLimits.ApartmentNumberMax"/>].
/// </summary>
public sealed class ApartmentNumber : ValueObject
{
    /// <summary>Underlying integer value.</summary>
    public int Value { get; }

    private ApartmentNumber(int value)
    {
        Value = value;
    }

    /// <summary>Creates an apartment number, throwing when out of range.</summary>
    /// <exception cref="ApartmentNumberOutOfRangeException">Thrown for any value outside the allowed range.</exception>
    public static ApartmentNumber From(int value)
    {
        if (value < PropertyLimits.ApartmentNumberMin || value > PropertyLimits.ApartmentNumberMax)
        {
            throw new ApartmentNumberOutOfRangeException(value);
        }

        return new ApartmentNumber(value);
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
