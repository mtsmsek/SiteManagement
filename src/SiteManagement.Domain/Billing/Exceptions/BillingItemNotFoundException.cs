using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Billing.Exceptions;

/// <summary>
/// Thrown when an item referenced for payment does not belong to the period.
/// </summary>
public sealed class BillingItemNotFoundException : DomainException
{
    /// <summary>Creates the exception for the missing item.</summary>
    public BillingItemNotFoundException(Guid itemId)
        : base(BillingMessageKeys.ItemNotFound, itemId)
    {
    }
}
