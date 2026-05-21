using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Billing.Exceptions;

/// <summary>
/// Thrown when a second billing item is added for an apartment that already
/// has one in the same period. Each apartment is billed once per period.
/// </summary>
public sealed class DuplicateBillingItemException : DomainException
{
    /// <summary>Creates the exception for the duplicated apartment.</summary>
    public DuplicateBillingItemException(Guid apartmentId)
        : base(BillingMessageKeys.DuplicateItem, apartmentId)
    {
    }
}
