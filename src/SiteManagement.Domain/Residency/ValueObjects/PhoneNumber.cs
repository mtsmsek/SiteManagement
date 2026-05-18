using System.Text.RegularExpressions;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Residency.ValueObjects;

/// <summary>
/// Turkish phone number. Accepts a handful of input forms (with or without
/// <c>+90</c>/<c>0</c>, with optional spaces/parens/dashes) and stores a
/// canonical <c>+90XXXXXXXXXX</c> shape so two different-looking inputs
/// compare equal.
/// </summary>
public sealed partial class PhoneNumber : ValueObject
{
    private const string CountryCode = "+90";
    private const int TrunkDigitCount = 10;

    /// <summary>Strip everything that isn't a digit or a leading <c>+</c>.</summary>
    [GeneratedRegex(@"[^\d+]")]
    private static partial Regex NoiseStripper();

    /// <summary>Canonical <c>+90XXXXXXXXXX</c> form.</summary>
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Parses the raw input, normalises it to <c>+90XXXXXXXXXX</c>, and
    /// throws when the result is not 10 trunk digits.
    /// </summary>
    /// <exception cref="InvalidPhoneNumberException">Thrown for any input that cannot be normalised to the expected shape.</exception>
    public static PhoneNumber From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidPhoneNumberException(raw ?? string.Empty);
        }

        if (raw.Length > ResidencyLimits.PhoneNumberMaxLength + 8)
        {
            // Even after stripping noise the result would be far too long to
            // be a phone number; reject early to avoid pointless work.
            throw new InvalidPhoneNumberException(raw);
        }

        var stripped = NoiseStripper().Replace(raw, string.Empty);

        var trunk = ExtractTrunkDigits(stripped, raw);
        return new PhoneNumber(CountryCode + trunk);
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>Pulls 10 trunk digits out of the stripped input, rejecting anything else.</summary>
    private static string ExtractTrunkDigits(string stripped, string rawForError)
    {
        if (stripped.StartsWith(CountryCode, StringComparison.Ordinal))
        {
            var trunk = stripped[CountryCode.Length..];
            return ValidateTrunk(trunk, rawForError);
        }

        if (stripped.StartsWith('+'))
        {
            // Some other country code — TR-only domain rule.
            throw new InvalidPhoneNumberException(rawForError);
        }

        if (stripped.StartsWith('0') && stripped.Length == TrunkDigitCount + 1)
        {
            return ValidateTrunk(stripped[1..], rawForError);
        }

        if (stripped.Length == TrunkDigitCount)
        {
            return ValidateTrunk(stripped, rawForError);
        }

        throw new InvalidPhoneNumberException(rawForError);
    }

    private static string ValidateTrunk(string trunk, string rawForError)
    {
        if (trunk.Length != TrunkDigitCount)
        {
            throw new InvalidPhoneNumberException(rawForError);
        }

        foreach (var c in trunk)
        {
            if (!char.IsDigit(c))
            {
                throw new InvalidPhoneNumberException(rawForError);
            }
        }

        return trunk;
    }
}
