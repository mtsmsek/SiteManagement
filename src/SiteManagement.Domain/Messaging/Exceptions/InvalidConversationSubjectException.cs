using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Messaging.Exceptions;

/// <summary>
/// Thrown when a conversation subject is empty / whitespace / longer than
/// <see cref="MessagingLimits.SubjectMaxLength"/>.
/// </summary>
public sealed class InvalidConversationSubjectException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidConversationSubjectException()
        : base(MessagingMessageKeys.SubjectInvalid, MessagingLimits.SubjectMaxLength)
    {
    }
}
