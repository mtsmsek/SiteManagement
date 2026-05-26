using SiteManagement.Domain.Messaging.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Messaging;

/// <summary>
/// Aggregate root for an admin ↔ resident message thread, scoped to one
/// resident. Either side opens it with a first message and both append replies;
/// reading marks the <em>other</em> side's messages as read, which the read
/// model turns into unread badges. Messages are inner entities reached only
/// through this root.
/// </summary>
public sealed class Conversation : AggregateRoot<Guid>
{
    private readonly List<Message> _messages = [];

    // EF Core materialisation ctor.
    private Conversation()
    {
        Subject = default!;
    }

    private Conversation(Guid id, Guid residentId, string subject) : base(id)
    {
        ResidentId = residentId;
        Subject = subject;
    }

    /// <summary>The resident this thread is with (Residency aggregate id).</summary>
    public Guid ResidentId { get; private set; }

    /// <summary>Short subject line.</summary>
    public string Subject { get; private set; }

    /// <summary>Read-only view over the messages, in send order.</summary>
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

    /// <summary>
    /// Opens a conversation with its first message. A thread always has at least
    /// one message, so opening and posting the opener are one atomic step.
    /// </summary>
    /// <exception cref="InvalidConversationSubjectException">Thrown when the subject is empty/whitespace/too long.</exception>
    /// <exception cref="InvalidMessageBodyException">Thrown when the opening body is invalid.</exception>
    public static Conversation Start(
        Guid residentId,
        string subject,
        MessageSenderRole openerRole,
        Guid openerUserId,
        string body,
        DateTime sentAtUtc)
    {
        if (string.IsNullOrWhiteSpace(subject) || subject.Length > MessagingLimits.SubjectMaxLength)
        {
            throw new InvalidConversationSubjectException();
        }

        var conversation = new Conversation(Guid.NewGuid(), residentId, subject.Trim());
        conversation._messages.Add(Message.Create(openerUserId, openerRole, body, sentAtUtc));
        return conversation;
    }

    /// <summary>
    /// Appends a reply and returns it, so the caller can register the brand-new
    /// inner entity with the persistence tracker (see the MarkAsAdded pattern).
    /// </summary>
    /// <exception cref="InvalidMessageBodyException">Thrown when the body is invalid.</exception>
    public Message PostMessage(Guid senderUserId, MessageSenderRole senderRole, string body, DateTime sentAtUtc)
    {
        var message = Message.Create(senderUserId, senderRole, body, sentAtUtc);
        _messages.Add(message);
        return message;
    }

    /// <summary>
    /// Marks every message authored by the <em>other</em> side as read at
    /// <paramref name="atUtc"/> — the reader doesn't "read" their own messages,
    /// and an already-read message keeps its original timestamp.
    /// </summary>
    public void MarkRead(MessageSenderRole reader, DateTime atUtc)
    {
        foreach (var message in _messages.Where(m => m.SenderRole != reader))
        {
            message.MarkRead(atUtc);
        }
    }
}
