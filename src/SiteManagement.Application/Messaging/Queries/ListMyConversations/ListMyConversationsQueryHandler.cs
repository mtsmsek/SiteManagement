using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;

namespace SiteManagement.Application.Messaging.Queries.ListMyConversations;

/// <summary>Scopes the inbox to the current resident's id (from the token, never the request).</summary>
public sealed class ListMyConversationsQueryHandler(ICurrentUser currentUser, IMessagingQueries messagingQueries)
    : IRequestHandler<ListMyConversationsQuery, IReadOnlyList<ConversationListItemDto>>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMessagingQueries _messagingQueries = messagingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<ConversationListItemDto>> Handle(ListMyConversationsQuery request, CancellationToken cancellationToken)
    {
        var residentId = _currentUser.ResidentId
            ?? throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);

        return _messagingQueries.ListForResidentAsync(residentId, cancellationToken);
    }
}
