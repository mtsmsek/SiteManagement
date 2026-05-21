namespace SiteManagement.Domain.Billing;

/// <summary>
/// Payment state of a single billing item (one apartment's share of a dues or
/// utility-bill period). Starts <see cref="Unpaid"/>; the Payment service
/// (W4) flips it to <see cref="Paid"/> once a transaction succeeds.
/// </summary>
public enum BillingItemStatus
{
    /// <summary>Not yet paid.</summary>
    Unpaid = 0,

    /// <summary>Settled by a successful payment.</summary>
    Paid = 1,
}
