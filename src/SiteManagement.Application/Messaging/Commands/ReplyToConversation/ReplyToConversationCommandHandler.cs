using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Notifications;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Messaging.Commands.ReplyToConversation;

/// <summary>
/// Appends the admin's reply to a loaded conversation. The new message is a
/// brand-new inner entity, so it is registered with the tracker via
/// <see cref="IUnitOfWork.MarkAsAdded{T}"/> (otherwise EF tries to UPDATE a row
/// that doesn't exist yet — the AddBlock/AddApartment pattern).
/// </summary>
public sealed class ReplyToConversationCommandHandler(
    IConversationRepository conversationRepository,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork,
    IMessagingNotifier notifier)
    : IRequestHandler<ReplyToConversationCommand>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingNotifier _notifier = notifier;

    /// <inheritdoc />
    public async Task Handle(ReplyToConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Conversation), request.ConversationId);

        var message = conversation.PostMessage(
            _currentUser.UserId, MessageSenderRole.Admin, request.Body, _timeProvider.GetUtcNow().UtcDateTime);

        _unitOfWork.MarkAsAdded(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The resident is the recipient of an admin reply — push to their stream.
        await _notifier.NotifyResidentAsync(
            conversation.ResidentId,
            new MessageReceivedNotification(conversation.Id),
            cancellationToken);
    }
}
