using MediatR;

namespace SiteManagement.Application.Messaging.Queries.ListConversations;

/// <summary>Delegates straight to <see cref="IMessagingQueries"/> — pure read path.</summary>
public sealed class ListConversationsQueryHandler(IMessagingQueries messagingQueries)
    : IRequestHandler<ListConversationsQuery, IReadOnlyList<ConversationListItemDto>>
{
    private readonly IMessagingQueries _messagingQueries = messagingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<ConversationListItemDto>> Handle(ListConversationsQuery request, CancellationToken cancellationToken)
        => _messagingQueries.ListAllAsync(cancellationToken);
}
