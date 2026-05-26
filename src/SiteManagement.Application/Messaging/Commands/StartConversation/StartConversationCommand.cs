using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Application.Messaging.Commands;

namespace SiteManagement.Application.Messaging.Commands.StartConversation;

/// <summary>
/// Admin opens a new conversation thread with a resident, supplying the first
/// message. Admin-only; the resident-initiated counterpart is
/// <c>StartMyConversationCommand</c>.
/// </summary>
public sealed record StartConversationCommand(Guid ResidentId, string Subject, string Body)
    : ICommand<ConversationCreatedResult>, IAdminRequest;
