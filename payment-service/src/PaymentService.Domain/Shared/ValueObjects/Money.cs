using System.Globalization;
using PaymentService.Domain.Shared.Exceptions;

namespace PaymentService.Domain.Shared.ValueObjects;

/// <summary>
/// A non-negative monetary amount in Turkish Lira. PaymentService keeps its own
/// copy (services share no code) — single-currency, 2-decimal rounding, value
/// equality, plus the comparison helper the balance checks need.
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>ISO 4217 code for the only currency this service handles.</summary>
    public const string TurkishLira = "TRY";

    private const int DecimalPlaces = 2;

    /// <summary>The rounded, non-negative amount.</summary>
    public decimal Amount { get; }

    /// <summary>Always <see cref="TurkishLira"/>.</summary>
    public string Currency => TurkishLira;

    private Money(decimal amount) => Amount = amount;

    /// <summary>A zero amount.</summary>
    public static Money Zero { get; } = new(0m);

    /// <summary>Creates a money value, rounding to two decimals (away from zero on a tie).</summary>
    /// <exception cref="NegativeMoneyException">Thrown when the amount is below zero.</exception>
    public static Money Of(decimal amount)
    {
        var rounded = Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);
        if (rounded < 0m)
        {
            throw new NegativeMoneyException(rounded);
        }

        return new Money(rounded);
    }

    /// <summary>Returns the sum of this and <paramref name="other"/>.</summary>
    public Money Add(Money other) => Of(Amount + other.Amount);

    /// <summary>Returns this minus <paramref name="other"/>.</summary>
    /// <exception cref="NegativeMoneyException">Thrown when the result would be negative.</exception>
    public Money Subtract(Money other) => Of(Amount - other.Amount);

    /// <summary>True when this amount is at least <paramref name="other"/> (the "enough balance?" check).</summary>
    public bool IsGreaterThanOrEqualTo(Money other) => Amount >= other.Amount;

    /// <inheritdoc />
    public override string ToString()
        => $"{Amount.ToString($"F{DecimalPlaces}", CultureInfo.InvariantCulture)} {Currency}";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
