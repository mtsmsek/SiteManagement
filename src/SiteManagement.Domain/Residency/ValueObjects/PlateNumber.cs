using System.Text.RegularExpressions;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Residency.ValueObjects;

/// <summary>
/// Turkish license plate (NN[A-Z]{1,3}NNNN with NN a valid 01..81 province
/// code). Casing is normalised to uppercase and stray whitespace/hyphens
/// are stripped at construction.
/// </summary>
public sealed partial class PlateNumber : ValueObject
{
    private const int MinProvinceCode = 1;
    private const int MaxProvinceCode = 81;

    /// <summary>NN + 1..3 letters + 2..4 digits.</summary>
    [GeneratedRegex(@"^(?<province>\d{2})(?<letters>[A-Z]{1,3})(?<digits>\d{2,4})$", RegexOptions.CultureInvariant)]
    private static partial Regex FormatPattern();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();

    /// <summary>Uppercase canonical form, no separators.</summary>
    public string Value { get; }

    private PlateNumber(string value)
    {
        Value = value;
    }

    /// <summary>Parses a raw plate, normalising case and stripping whitespace.</summary>
    /// <exception cref="InvalidPlateNumberException">Thrown when the cleaned value does not match the plate shape or has an invalid province code.</exception>
    public static PlateNumber From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidPlateNumberException(raw ?? string.Empty);
        }

        var cleaned = WhitespacePattern().Replace(raw, string.Empty).ToUpperInvariant();

        var match = FormatPattern().Match(cleaned);
        if (!match.Success)
        {
            throw new InvalidPlateNumberException(raw);
        }

        var province = int.Parse(match.Groups["province"].Value);
        if (province < MinProvinceCode || province > MaxProvinceCode)
        {
            throw new InvalidPlateNumberException(raw);
        }

        return new PlateNumber(cleaned);
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
