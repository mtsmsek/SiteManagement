using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Messaging.Commands.ReplyToConversation;

/// <summary>Admin appends a reply to an existing conversation. Admin-only.</summary>
public sealed record ReplyToConversationCommand(Guid ConversationId, string Body) : ICommand, IAdminRequest;
