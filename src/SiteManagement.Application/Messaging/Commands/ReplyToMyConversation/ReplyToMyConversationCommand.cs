using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Commands.ReplyToMyConversation;

/// <summary>
/// Resident appends a reply to one of their own conversations. As an
/// <see cref="IOwnedConversationRequest"/>, ownership of
/// <see cref="ConversationId"/> is verified by the pipeline before the handler
/// runs, so the handler carries no authorization code.
/// </summary>
public sealed record ReplyToMyConversationCommand(Guid ConversationId, string Body)
    : ICommand, IOwnedConversationRequest;
