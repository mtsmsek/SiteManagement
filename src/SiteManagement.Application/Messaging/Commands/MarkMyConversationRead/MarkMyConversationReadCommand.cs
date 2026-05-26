using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Commands.MarkMyConversationRead;

/// <summary>
/// Resident marks one of their own conversations read, clearing the unread badge
/// on the admin's messages. Ownership of <see cref="ConversationId"/> is verified
/// by the pipeline (<see cref="IOwnedConversationRequest"/>).
/// </summary>
public sealed record MarkMyConversationReadCommand(Guid ConversationId)
    : ICommand, IOwnedConversationRequest;
