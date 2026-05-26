using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Commands.MarkConversationRead;

/// <summary>Admin marks a conversation read, clearing the unread badge on the resident's messages. Admin-only.</summary>
public sealed record MarkConversationReadCommand(Guid ConversationId) : ICommand, IAdminRequest;
