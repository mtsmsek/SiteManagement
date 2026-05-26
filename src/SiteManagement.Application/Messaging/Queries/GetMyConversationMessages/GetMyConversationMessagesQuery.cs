using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Queries.GetMyConversationMessages;

/// <summary>
/// Resident reads one of their own conversations' messages. As an
/// <see cref="IOwnedConversationRequest"/>, ownership is verified by the pipeline
/// before the handler runs.
/// </summary>
public sealed record GetMyConversationMessagesQuery(Guid ConversationId)
    : IQuery<IReadOnlyList<MessageDto>>, IOwnedConversationRequest;
