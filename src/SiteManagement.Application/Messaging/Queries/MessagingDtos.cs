namespace SiteManagement.Application.Messaging.Queries;

/// <summary>
/// One conversation row for an inbox list. Carries per-side unread counts so the
/// admin inbox and the resident portal can each show their own badge from a
/// single projection.
/// </summary>
public sealed record ConversationListItemDto(
    Guid Id,
    Guid ResidentId,
    string Subject,
    int MessageCount,
    DateTime LastMessageAtUtc,
    int UnreadForAdmin,
    int UnreadForResident);

/// <summary>One message within a conversation thread.</summary>
public sealed record MessageDto(
    Guid Id,
    string SenderRole,
    string Body,
    DateTime SentAtUtc,
    DateTime? ReadAtUtc);
