namespace SiteManagement.Application.Messaging.Queries;

/// <summary>
/// Read-side projections for the Messaging context. Returns DTOs only (never
/// domain entities), <c>AsNoTracking</c>. Admin sees every conversation; a
/// resident sees only their own (the resident id comes from the token, never
/// the request).
/// </summary>
public interface IMessagingQueries
{
    /// <summary>Every conversation, most recently active first (admin inbox).</summary>
    Task<IReadOnlyList<ConversationListItemDto>> ListAllAsync(CancellationToken ct = default);

    /// <summary>The given resident's own conversations, most recently active first.</summary>
    Task<IReadOnlyList<ConversationListItemDto>> ListForResidentAsync(Guid residentId, CancellationToken ct = default);

    /// <summary>The messages of one conversation, in send order.</summary>
    Task<IReadOnlyList<MessageDto>> ListMessagesAsync(Guid conversationId, CancellationToken ct = default);

    /// <summary>
    /// The resident a conversation belongs to, or <c>null</c> if it doesn't
    /// exist. Used by the ownership behavior to authorize resident access.
    /// </summary>
    Task<Guid?> FindResidentIdAsync(Guid conversationId, CancellationToken ct = default);
}
