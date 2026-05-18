using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Property.ValueObjects;

/// <summary>
/// Display name of a block (e.g. <c>"A"</c>, <c>"Block-7"</c>). Stored as the
/// caller's original casing for display, but equality is case-insensitive so
/// duplicate-name detection works regardless of how the user typed it.
/// </summary>
public sealed class BlockName : ValueObject
{
    /// <summary>Trimmed display value, in the casing the caller supplied.</summary>
    public string Value { get; }

    private BlockName(string value)
    {
        Value = value;
    }

    /// <summary>Creates a block name, throwing when empty or too long.</summary>
    /// <exception cref="InvalidBlockNameException">Thrown when the trimmed value is empty or longer than the allowed maximum.</exception>
    public static BlockName From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidBlockNameException();
        }

        var trimmed = value.Trim();
        if (trimmed.Length > PropertyLimits.BlockNameMaxLength)
        {
            throw new InvalidBlockNameException();
        }

        return new BlockName(trimmed);
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToUpperInvariant();
    }
}
