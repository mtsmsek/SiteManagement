using MediatR;

namespace SiteManagement.Application.Messaging.Queries.GetMyConversationMessages;

/// <summary>
/// Returns the conversation's messages. Ownership is already enforced by the
/// pipeline (the query is an <c>IOwnedConversationRequest</c>), so this is a pure
/// read delegating to <see cref="IMessagingQueries"/>.
/// </summary>
public sealed class GetMyConversationMessagesQueryHandler(IMessagingQueries messagingQueries)
    : IRequestHandler<GetMyConversationMessagesQuery, IReadOnlyList<MessageDto>>
{
    private readonly IMessagingQueries _messagingQueries = messagingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<MessageDto>> Handle(GetMyConversationMessagesQuery request, CancellationToken cancellationToken)
        => _messagingQueries.ListMessagesAsync(request.ConversationId, cancellationToken);
}
