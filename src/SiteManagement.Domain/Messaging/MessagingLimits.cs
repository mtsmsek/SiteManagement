namespace SiteManagement.Domain.Messaging;

/// <summary>
/// Length limits for the Messaging bounded context, kept in one place so the
/// aggregate invariants, the EF column widths, and any Application validators
/// all agree on the same numbers.
/// </summary>
public static class MessagingLimits
{
    /// <summary>Maximum length of a conversation subject.</summary>
    public const int SubjectMaxLength = 150;

    /// <summary>Maximum length of a single message body.</summary>
    public const int BodyMaxLength = 2000;

    /// <summary>Max stored length of <see cref="MessageSenderRole"/> as a string.</summary>
    public const int SenderRoleMaxLength = 20;
}
