using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Notifications;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Messaging.Commands.MarkMyConversationRead;

/// <summary>
/// Marks the admin's messages in the resident's own thread as read. Ownership is
/// enforced by the pipeline (<c>IOwnedConversationRequest</c>).
/// </summary>
public sealed class MarkMyConversationReadCommandHandler(
    IConversationRepository conversationRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork,
    IMessagingNotifier notifier)
    : IRequestHandler<MarkMyConversationReadCommand>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingNotifier _notifier = notifier;

    /// <inheritdoc />
    public async Task Handle(MarkMyConversationReadCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Conversation), request.ConversationId);

        conversation.MarkRead(MessageSenderRole.Resident, _timeProvider.GetUtcNow().UtcDateTime);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Tell admins the resident has read their messages — clears the badge on the admin inbox.
        await _notifier.NotifyAdminsAsync(
            new MessageReadNotification(conversation.Id),
            cancellationToken);
    }
}
