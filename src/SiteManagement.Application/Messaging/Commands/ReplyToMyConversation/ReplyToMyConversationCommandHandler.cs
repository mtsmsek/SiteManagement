using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Notifications;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Messaging.Commands.ReplyToMyConversation;

/// <summary>
/// Appends the resident's reply to their own conversation. Ownership is enforced
/// by the pipeline (the command is an <c>IOwnedConversationRequest</c>), so this
/// handler is pure messaging work. The new message is registered with the
/// tracker via <see cref="IUnitOfWork.MarkAsAdded{T}"/>.
/// </summary>
public sealed class ReplyToMyConversationCommandHandler(
    IConversationRepository conversationRepository,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork,
    IMessagingNotifier notifier)
    : IRequestHandler<ReplyToMyConversationCommand>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingNotifier _notifier = notifier;

    /// <inheritdoc />
    public async Task Handle(ReplyToMyConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Conversation), request.ConversationId);

        var message = conversation.PostMessage(
            _currentUser.UserId, MessageSenderRole.Resident, request.Body, _timeProvider.GetUtcNow().UtcDateTime);

        _unitOfWork.MarkAsAdded(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Admins should see the new reply in their inbox in real time.
        await _notifier.NotifyAdminsAsync(
            new MessageReceivedNotification(conversation.Id),
            cancellationToken);
    }
}
