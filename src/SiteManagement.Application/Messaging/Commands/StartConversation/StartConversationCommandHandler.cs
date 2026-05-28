using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Commands;
using SiteManagement.Application.Messaging.Notifications;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Messaging.Commands.StartConversation;

/// <summary>Opens the conversation with the admin's first message and persists it.</summary>
public sealed class StartConversationCommandHandler(
    IConversationRepository conversationRepository,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork,
    IMessagingNotifier notifier)
    : IRequestHandler<StartConversationCommand, ConversationCreatedResult>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingNotifier _notifier = notifier;

    /// <inheritdoc />
    public async Task<ConversationCreatedResult> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = Conversation.Start(
            request.ResidentId,
            request.Subject,
            MessageSenderRole.Admin,
            _currentUser.UserId,
            request.Body,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify the resident's open tabs (if any) that a brand-new thread exists.
        await _notifier.NotifyResidentAsync(
            conversation.ResidentId,
            new ConversationStartedNotification(conversation.Id, conversation.ResidentId),
            cancellationToken);

        return new ConversationCreatedResult(conversation.Id);
    }
}
