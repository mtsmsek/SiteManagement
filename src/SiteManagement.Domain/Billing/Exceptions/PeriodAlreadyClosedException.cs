using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Billing.Exceptions;

/// <summary>
/// Thrown when a billing period that is already closed is mutated — adding an
/// item, distributing, or closing it again. A closed period is immutable.
/// </summary>
public sealed class PeriodAlreadyClosedException : DomainException
{
    /// <summary>Creates the exception for the closed period.</summary>
    public PeriodAlreadyClosedException(Guid periodId)
        : base(BillingMessageKeys.PeriodAlreadyClosed, periodId)
    {
    }
}
