using MediatR;

namespace SiteManagement.Application.Messaging.Queries.GetConversationMessages;

/// <summary>Delegates straight to <see cref="IMessagingQueries"/> — pure read path.</summary>
public sealed class GetConversationMessagesQueryHandler(IMessagingQueries messagingQueries)
    : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<MessageDto>>
{
    private readonly IMessagingQueries _messagingQueries = messagingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
        => _messagingQueries.ListMessagesAsync(request.ConversationId, cancellationToken);
}
