using System.Globalization;
using SiteManagement.Domain.Billing.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Billing.ValueObjects;

/// <summary>
/// The calendar month a billing period covers, e.g. 2026-05. Stored as a
/// year + month pair (no day, no time) because dues and utility bills are
/// reasoned about per month. Rendered as "yyyy-MM" and ordered chronologically.
/// </summary>
public sealed class BillingMonth : ValueObject, IComparable<BillingMonth>
{
    private const int MinYear = 1900;
    private const int MaxYear = 3000;
    private const int MinMonth = 1;
    private const int MaxMonth = 12;

    /// <summary>Four-digit calendar year.</summary>
    public int Year { get; }

    /// <summary>Calendar month, 1..12.</summary>
    public int Month { get; }

    private BillingMonth(int year, int month)
    {
        Year = year;
        Month = month;
    }

    /// <summary>Creates a billing month, validating year and month ranges.</summary>
    /// <exception cref="InvalidBillingMonthException">Thrown for an out-of-range year or month.</exception>
    public static BillingMonth Of(int year, int month)
    {
        if (year is < MinYear or > MaxYear || month is < MinMonth or > MaxMonth)
        {
            throw new InvalidBillingMonthException(year, month);
        }

        return new BillingMonth(year, month);
    }

    /// <inheritdoc />
    public int CompareTo(BillingMonth? other)
    {
        if (other is null)
        {
            return 1;
        }

        var byYear = Year.CompareTo(other.Year);
        return byYear != 0 ? byYear : Month.CompareTo(other.Month);
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{Year.ToString("D4", CultureInfo.InvariantCulture)}-{Month.ToString("D2", CultureInfo.InvariantCulture)}";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Year;
        yield return Month;
    }
}
