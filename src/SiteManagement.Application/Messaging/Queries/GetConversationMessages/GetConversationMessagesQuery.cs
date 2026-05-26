using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Queries.GetConversationMessages;

/// <summary>Admin reads one conversation's messages, in send order.</summary>
public sealed record GetConversationMessagesQuery(Guid ConversationId)
    : IQuery<IReadOnlyList<MessageDto>>, IAdminRequest;
