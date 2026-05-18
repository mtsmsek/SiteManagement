using System.Text.RegularExpressions;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Residency.ValueObjects;

/// <summary>
/// Email address. The Domain enforces a deliberately simple
/// <c>local@domain.tld</c> shape — overly strict regex'es reject too many
/// real-world addresses, so the rule is "non-empty local, single '@', and
/// at least one dot in the domain". Storage form is lowercase so equality
/// matches the SMTP case-insensitivity contract.
/// </summary>
public sealed partial class Email : ValueObject
{
    [GeneratedRegex(@"^[^\s@]+@[^\s@.]+\.[^\s@]+$", RegexOptions.CultureInvariant)]
    private static partial Regex FormatPattern();

    /// <summary>Lower-cased storage form.</summary>
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>Creates an email value object after shape + length validation.</summary>
    /// <exception cref="InvalidEmailException">Thrown for null/empty input, bad shape, or oversized strings.</exception>
    public static Email From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidEmailException(raw ?? string.Empty);
        }

        var trimmed = raw.Trim();
        if (trimmed.Length > ResidencyLimits.EmailMaxLength || !FormatPattern().IsMatch(trimmed))
        {
            throw new InvalidEmailException(raw);
        }

        return new Email(trimmed.ToLowerInvariant());
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
