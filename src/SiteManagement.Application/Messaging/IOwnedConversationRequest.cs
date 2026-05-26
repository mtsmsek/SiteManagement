using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging;

/// <summary>
/// A resident request that acts on a specific conversation. The
/// <c>ConversationOwnershipBehavior</c> verifies the conversation belongs to the
/// caller before the handler runs, so resource ownership (the IDOR guard) is
/// enforced centrally — the handler never re-checks it. Extends
/// <see cref="IResidentRequest"/>, so the role gate still applies first.
/// </summary>
public interface IOwnedConversationRequest : IResidentRequest
{
    /// <summary>The conversation the resident is acting on.</summary>
    Guid ConversationId { get; }
}
