using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Commands;
using SiteManagement.Application.Messaging.Notifications;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Messaging.Commands.StartMyConversation;

/// <summary>Opens the conversation with the resident's first message, scoped to the caller.</summary>
public sealed class StartMyConversationCommandHandler(
    IConversationRepository conversationRepository,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork,
    IMessagingNotifier notifier)
    : IRequestHandler<StartMyConversationCommand, ConversationCreatedResult>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingNotifier _notifier = notifier;

    /// <inheritdoc />
    public async Task<ConversationCreatedResult> Handle(StartMyConversationCommand request, CancellationToken cancellationToken)
    {
        var residentId = _currentUser.ResidentId
            ?? throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);

        var conversation = Conversation.Start(
            residentId,
            request.Subject,
            MessageSenderRole.Resident,
            _currentUser.UserId,
            request.Body,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Every admin's inbox should learn about the new thread.
        await _notifier.NotifyAdminsAsync(
            new ConversationStartedNotification(conversation.Id, residentId),
            cancellationToken);

        return new ConversationCreatedResult(conversation.Id);
    }
}
