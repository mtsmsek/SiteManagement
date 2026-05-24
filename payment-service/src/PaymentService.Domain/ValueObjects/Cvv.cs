using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared;

namespace PaymentService.Domain.ValueObjects;

/// <summary>
/// Card security code: 3 digits (Visa/Mastercard) or 4 (Amex). Stored only for
/// the duration of a charge in this fake gateway; a real one would never persist it.
/// </summary>
public sealed class Cvv : ValueObject
{
    private const int MinLength = 3;
    private const int MaxLength = 4;

    /// <summary>The 3-4 digit code.</summary>
    public string Value { get; }

    private Cvv(string value) => Value = value;

    /// <summary>Creates a CVV, validating length + digits.</summary>
    /// <exception cref="InvalidCardException">Thrown when not 3-4 digits.</exception>
    public static Cvv From(string raw)
    {
        var trimmed = (raw ?? string.Empty).Trim();

        if (trimmed.Length is < MinLength or > MaxLength || !trimmed.All(char.IsDigit))
        {
            throw new InvalidCardException("cvv");
        }

        return new Cvv(trimmed);
    }

    /// <summary>Never reveal the code in logs.</summary>
    public override string ToString() => "***";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
