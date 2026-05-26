using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Application.Messaging.Commands;

namespace SiteManagement.Application.Messaging.Commands.StartMyConversation;

/// <summary>
/// Resident opens a new conversation thread for themselves (e.g. reports an
/// issue), supplying the first message. The resident id comes from the token,
/// never the request, so the thread is always scoped to the caller.
/// </summary>
public sealed record StartMyConversationCommand(string Subject, string Body)
    : ICommand<ConversationCreatedResult>, IResidentRequest;
