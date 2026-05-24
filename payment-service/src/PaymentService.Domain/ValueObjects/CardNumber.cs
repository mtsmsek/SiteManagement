using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared;

namespace PaymentService.Domain.ValueObjects;

/// <summary>
/// A 16-digit primary account number (PAN) that passes the Luhn checksum —
/// the same check real card networks use to reject typos. Spaces are stripped
/// so grouped input ("4242 4242 ...") is accepted.
/// </summary>
public sealed class CardNumber : ValueObject
{
    private const int PanLength = 16;

    /// <summary>The 16-digit value (no spaces).</summary>
    public string Value { get; }

    private CardNumber(string value) => Value = value;

    /// <summary>Creates a card number, validating length, digits, and the Luhn checksum.</summary>
    /// <exception cref="InvalidCardException">Thrown for any input that fails validation.</exception>
    public static CardNumber From(string raw)
    {
        var trimmed = (raw ?? string.Empty).Replace(" ", string.Empty);

        if (trimmed.Length != PanLength || !trimmed.All(char.IsDigit) || !PassesLuhn(trimmed))
        {
            throw new InvalidCardException("card number");
        }

        return new CardNumber(trimmed);
    }

    /// <summary>Standard Luhn (mod-10) checksum.</summary>
    private static bool PassesLuhn(string digits)
    {
        var sum = 0;
        var doubleNext = false;
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var d = digits[i] - '0';
            if (doubleNext)
            {
                d *= 2;
                if (d > 9)
                {
                    d -= 9;
                }
            }

            sum += d;
            doubleNext = !doubleNext;
        }

        return sum % 10 == 0;
    }

    /// <summary>Masked form for logging/storage — only the last four digits survive.</summary>
    public override string ToString() => $"**** **** **** {Value[^4..]}";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
