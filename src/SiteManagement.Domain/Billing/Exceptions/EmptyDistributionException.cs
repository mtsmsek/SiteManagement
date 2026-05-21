using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Billing.Exceptions;

/// <summary>
/// Thrown when a utility bill is distributed across zero apartments — there is
/// nothing to split the total between, so the distribution is meaningless.
/// </summary>
public sealed class EmptyDistributionException : DomainException
{
    /// <summary>Creates the exception for the empty distribution.</summary>
    public EmptyDistributionException()
        : base(BillingMessageKeys.EmptyDistribution)
    {
    }
}
