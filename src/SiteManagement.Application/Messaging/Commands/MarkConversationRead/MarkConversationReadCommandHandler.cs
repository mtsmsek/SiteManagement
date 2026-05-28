using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Notifications;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Messaging.Commands.MarkConversationRead;

/// <summary>Marks the resident's messages in the thread as read by the admin.</summary>
public sealed class MarkConversationReadCommandHandler(
    IConversationRepository conversationRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork,
    IMessagingNotifier notifier)
    : IRequestHandler<MarkConversationReadCommand>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingNotifier _notifier = notifier;

    /// <inheritdoc />
    public async Task Handle(MarkConversationReadCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Conversation), request.ConversationId);

        conversation.MarkRead(MessageSenderRole.Admin, _timeProvider.GetUtcNow().UtcDateTime);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Let the resident know their messages have been read (clears the badge on their side).
        await _notifier.NotifyResidentAsync(
            conversation.ResidentId,
            new MessageReadNotification(conversation.Id),
            cancellationToken);
    }
}
