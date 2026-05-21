using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Billing.Exceptions;

/// <summary>
/// Thrown when a <see cref="SiteManagement.Domain.Billing.ValueObjects.BillingMonth"/>
/// is created with a month outside 1..12 or a year outside the supported range.
/// </summary>
public sealed class InvalidBillingMonthException : DomainException
{
    /// <summary>Creates the exception for the offending year/month.</summary>
    public InvalidBillingMonthException(int year, int month)
        : base(BillingMessageKeys.BillingMonthInvalid, year, month)
    {
    }
}
