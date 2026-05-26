using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Messaging.Exceptions;

/// <summary>
/// Thrown when a message body is empty / whitespace / longer than
/// <see cref="MessagingLimits.BodyMaxLength"/>.
/// </summary>
public sealed class InvalidMessageBodyException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidMessageBodyException()
        : base(MessagingMessageKeys.BodyInvalid, MessagingLimits.BodyMaxLength)
    {
    }
}
