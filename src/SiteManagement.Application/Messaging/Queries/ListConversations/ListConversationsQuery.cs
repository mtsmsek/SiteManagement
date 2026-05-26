using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Queries.ListConversations;

/// <summary>Admin inbox: every conversation, most recently active first.</summary>
public sealed record ListConversationsQuery : IQuery<IReadOnlyList<ConversationListItemDto>>, IAdminRequest;
