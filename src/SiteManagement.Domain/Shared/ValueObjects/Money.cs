using System.Globalization;
using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Shared.ValueObjects;

/// <summary>
/// A non-negative monetary amount in Turkish Lira. The domain is
/// single-currency (the PDF requires no FX), so currency is fixed and
/// arithmetic never has to reconcile two currencies. Amounts are rounded to
/// two decimals at construction; results of arithmetic stay non-negative
/// (debts are modelled as billing items, not negative money).
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>ISO 4217 code for the only currency this domain handles.</summary>
    public const string TurkishLira = "TRY";

    private const int DecimalPlaces = 2;

    /// <summary>The rounded, non-negative amount.</summary>
    public decimal Amount { get; }

    /// <summary>Always <see cref="TurkishLira"/> in this domain.</summary>
    public string Currency => TurkishLira;

    private Money(decimal amount)
    {
        Amount = amount;
    }

    /// <summary>A zero amount.</summary>
    public static Money Zero { get; } = new(0m);

    /// <summary>
    /// Creates a money value, rounding to two decimals (away from zero on a
    /// tie, the everyday cash-rounding convention).
    /// </summary>
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

    /// <summary>
    /// Returns this minus <paramref name="other"/>.
    /// </summary>
    /// <exception cref="NegativeMoneyException">Thrown when the result would be negative.</exception>
    public Money Subtract(Money other) => Of(Amount - other.Amount);

    /// <summary>Returns this amount scaled by a non-negative integer factor.</summary>
    public Money Multiply(int factor)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(factor);
        return Of(Amount * factor);
    }

    /// <summary>
    /// Splits this amount into <paramref name="count"/> equal shares. Each
    /// share is rounded down to two decimals; the rounding remainder is folded
    /// into the last share so the shares always sum back to the original total
    /// (no lost or invented kuruş).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is not positive.</exception>
    public IReadOnlyList<Money> DistributeEqually(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        var baseShare = Math.Floor(Amount / count * 100m) / 100m;
        var shares = new Money[count];
        var distributed = 0m;
        for (var i = 0; i < count - 1; i++)
        {
            shares[i] = new Money(baseShare);
            distributed += baseShare;
        }

        shares[count - 1] = new Money(Amount - distributed);
        return shares;
    }

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
