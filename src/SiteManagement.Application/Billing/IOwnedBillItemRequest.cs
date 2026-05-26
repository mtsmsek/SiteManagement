using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing;

/// <summary>
/// A resident request that acts on a specific billing item. The
/// <c>ResidentBillOwnershipBehavior</c> verifies the item is one of the caller's
/// own bills before the handler runs, so resource ownership (the IDOR guard) is
/// enforced centrally — the handler never re-checks it. Extends
/// <see cref="IResidentRequest"/>, so the role gate still applies first.
/// </summary>
public interface IOwnedBillItemRequest : IResidentRequest
{
    /// <summary>The billing item the resident is acting on.</summary>
    Guid ItemId { get; }
}
