using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared;

namespace PaymentService.Domain.ValueObjects;

/// <summary>
/// A card's expiry month/year. A card is valid through the last day of its
/// expiry month; expiry is evaluated against a caller-supplied reference date
/// so the rule is deterministic and unit-testable (no hidden clock).
/// </summary>
public sealed class ExpiryDate : ValueObject
{
    private const int MinMonth = 1;
    private const int MaxMonth = 12;

    /// <summary>Four-digit year.</summary>
    public int Year { get; }

    /// <summary>Month, 1..12.</summary>
    public int Month { get; }

    private ExpiryDate(int year, int month)
    {
        Year = year;
        Month = month;
    }

    /// <summary>Creates an expiry, validating the month range.</summary>
    /// <exception cref="InvalidCardException">Thrown when month is outside 1..12.</exception>
    public static ExpiryDate Of(int year, int month)
    {
        if (month is < MinMonth or > MaxMonth)
        {
            throw new InvalidCardException("expiry");
        }

        return new ExpiryDate(year, month);
    }

    /// <summary>True when <paramref name="asOf"/> is past the last day of the expiry month.</summary>
    public bool IsExpired(DateOnly asOf)
    {
        var lastDay = new DateOnly(Year, Month, DateTime.DaysInMonth(Year, Month));
        return asOf > lastDay;
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Year;
        yield return Month;
    }
}
