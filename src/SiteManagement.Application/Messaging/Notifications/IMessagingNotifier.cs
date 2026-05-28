namespace SiteManagement.Application.Messaging.Notifications;

/// <summary>
/// Port for pushing ephemeral, real-time notifications about messaging activity
/// to connected clients. Implemented by the API layer over SignalR. Distinct
/// from <c>IIntegrationEvent</c>: this is fire-and-forget UI hint, not durable
/// delivery — the Outbox path stays the home of guaranteed work (welcome mail
/// etc.). Each call targets either the admin audience or one resident.
/// </summary>
public interface IMessagingNotifier
{
    /// <summary>Push a notification to every connected admin client.</summary>
    Task NotifyAdminsAsync(IMessagingNotification payload, CancellationToken cancellationToken = default);

    /// <summary>Push a notification to the resident's own connected clients.</summary>
    Task NotifyResidentAsync(Guid residentId, IMessagingNotification payload, CancellationToken cancellationToken = default);
}

/// <summary>
/// A messaging push payload. <see cref="EventName"/> is the SignalR method the
/// client listens on; the record itself is the JSON payload.
/// </summary>
public interface IMessagingNotification
{
    /// <summary>The SignalR method name the client subscribed to.</summary>
    string EventName { get; }
}

/// <summary>
/// "A new message landed in this conversation" — the client refetches the
/// thread + inbox. Minimal payload by design: the client already knows how
/// to fetch the canonical state, and we avoid drift between push payload
/// and the read projection.
/// </summary>
public sealed record MessageReceivedNotification(Guid ConversationId) : IMessagingNotification
{
    /// <inheritdoc />
    public string EventName => "MessageReceived";
}

/// <summary>
/// "A brand-new conversation now exists" — the client refetches the inbox.
/// Sent to the side that did not originate the thread.
/// </summary>
public sealed record ConversationStartedNotification(Guid ConversationId, Guid ResidentId) : IMessagingNotification
{
    /// <inheritdoc />
    public string EventName => "ConversationStarted";
}

/// <summary>
/// "The other side just read your messages" — the client refetches the inbox
/// to clear the unread badge for that side.
/// </summary>
public sealed record MessageReadNotification(Guid ConversationId) : IMessagingNotification
{
    /// <inheritdoc />
    public string EventName => "MessageRead";
}
