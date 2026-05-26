using SiteManagement.Domain.Messaging.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Messaging;

/// <summary>
/// A single message inside a <see cref="Conversation"/>. An inner entity of the
/// conversation aggregate — created and mutated only through the root, never on
/// its own. Identified by id; immutable except for the read receipt.
/// </summary>
public sealed class Message : Entity<Guid>
{
    // EF Core materialisation ctor.
    private Message()
    {
        Body = default!;
    }

    private Message(Guid id, Guid senderUserId, MessageSenderRole senderRole, string body, DateTime sentAtUtc)
        : base(id)
    {
        SenderUserId = senderUserId;
        SenderRole = senderRole;
        Body = body;
        SentAtUtc = sentAtUtc;
    }

    /// <summary>The Identity user id that authored the message.</summary>
    public Guid SenderUserId { get; private set; }

    /// <summary>Which side of the conversation sent it.</summary>
    public MessageSenderRole SenderRole { get; private set; }

    /// <summary>The message text.</summary>
    public string Body { get; private set; }

    /// <summary>When the message was sent (UTC, supplied by the caller's clock).</summary>
    public DateTime SentAtUtc { get; private set; }

    /// <summary>When the other side first read it; <c>null</c> while unread.</summary>
    public DateTime? ReadAtUtc { get; private set; }

    /// <summary>Creates a validated message. Internal: only <see cref="Conversation"/> calls it.</summary>
    /// <exception cref="InvalidMessageBodyException">Thrown when the body is empty/whitespace/too long.</exception>
    internal static Message Create(Guid senderUserId, MessageSenderRole senderRole, string body, DateTime sentAtUtc)
    {
        if (string.IsNullOrWhiteSpace(body) || body.Length > MessagingLimits.BodyMaxLength)
        {
            throw new InvalidMessageBodyException();
        }

        return new Message(Guid.NewGuid(), senderUserId, senderRole, body.Trim(), sentAtUtc);
    }

    /// <summary>Stamps the read receipt the first time the other side reads it; a no-op afterwards.</summary>
    internal void MarkRead(DateTime atUtc)
    {
        ReadAtUtc ??= atUtc;
    }
}
