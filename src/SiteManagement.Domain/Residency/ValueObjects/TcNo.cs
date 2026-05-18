using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Residency.ValueObjects;

/// <summary>
/// Turkish citizenship number (T.C. Kimlik No). Validated against the
/// official 11-digit checksum algorithm at construction time so the rest of
/// the domain can rely on every <see cref="TcNo"/> being well-formed.
/// </summary>
public sealed class TcNo : ValueObject
{
    /// <summary>The 11-digit value.</summary>
    public string Value { get; }

    private TcNo(string value)
    {
        Value = value;
    }

    /// <summary>Creates a TcNo, validating shape + leading digit + checksum.</summary>
    /// <exception cref="InvalidTcNoException">Thrown for any input that fails validation.</exception>
    public static TcNo From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidTcNoException();
        }

        var trimmed = raw.Trim();
        if (!IsValidShape(trimmed) || !ChecksumMatches(trimmed))
        {
            throw new InvalidTcNoException();
        }

        return new TcNo(trimmed);
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>11 digits, first digit non-zero.</summary>
    private static bool IsValidShape(string value)
    {
        if (value.Length != ResidencyLimits.TcNoLength)
        {
            return false;
        }

        if (value[0] == '0')
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Verifies the two checksum digits using the official algorithm:
    /// <c>d10 = ((odd_sum * 7) - even_sum) mod 10</c> and
    /// <c>d11 = (sum of d1..d10) mod 10</c>.
    /// </summary>
    private static bool ChecksumMatches(string value)
    {
        Span<int> d = stackalloc int[ResidencyLimits.TcNoLength];
        for (var i = 0; i < d.Length; i++)
        {
            d[i] = value[i] - '0';
        }

        var oddSum = d[0] + d[2] + d[4] + d[6] + d[8];
        var evenSum = d[1] + d[3] + d[5] + d[7];

        var expected10 = ((oddSum * 7) - evenSum) % 10;
        if (expected10 < 0)
        {
            expected10 += 10;
        }
        if (d[9] != expected10)
        {
            return false;
        }

        var firstTenSum = 0;
        for (var i = 0; i < 10; i++)
        {
            firstTenSum += d[i];
        }
        var expected11 = firstTenSum % 10;
        return d[10] == expected11;
    }
}
