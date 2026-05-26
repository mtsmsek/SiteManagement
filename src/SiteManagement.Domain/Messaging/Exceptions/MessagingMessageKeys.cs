namespace SiteManagement.Domain.Messaging.Exceptions;

/// <summary>
/// Stable resource keys carried by Messaging-context domain exceptions. The
/// Application layer's <c>ExceptionTranslationBehavior</c> looks each key up
/// against the <c>ErrorMessages</c> resource bundle when translating a
/// <see cref="SiteManagement.Domain.Shared.Exceptions.DomainException"/> into a
/// localized <c>BusinessRuleViolationException</c>.
/// </summary>
public static class MessagingMessageKeys
{
    /// <summary><c>"Messaging.Conversation.SubjectInvalid"</c> — empty/whitespace/over-long subject.</summary>
    public const string SubjectInvalid = "Messaging.Conversation.SubjectInvalid";

    /// <summary><c>"Messaging.Message.BodyInvalid"</c> — empty/whitespace/over-long message body.</summary>
    public const string BodyInvalid = "Messaging.Message.BodyInvalid";
}
