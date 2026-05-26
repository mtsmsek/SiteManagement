using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Commands;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Messaging.Commands.StartConversation;

/// <summary>Opens the conversation with the admin's first message and persists it.</summary>
public sealed class StartConversationCommandHandler(
    IConversationRepository conversationRepository,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<StartConversationCommand, ConversationCreatedResult>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

        return new ConversationCreatedResult(conversation.Id);
    }
}
