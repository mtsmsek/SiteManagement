using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Queries.ListMyConversations;

/// <summary>Resident inbox: the caller's own conversations, most recently active first.</summary>
public sealed record ListMyConversationsQuery : IQuery<IReadOnlyList<ConversationListItemDto>>, IResidentRequest;
