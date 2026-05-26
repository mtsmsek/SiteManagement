using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Messaging;
using SiteManagement.Application.Messaging.Queries;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;

namespace SiteManagement.Application.Behaviors;

/// <summary>
/// Resource-based authorization for resident messaging: for any
/// <see cref="IOwnedConversationRequest"/>, verifies the conversation belongs to
/// the caller before the handler runs (else 403). Mirrors
/// <see cref="ResidentBillOwnershipBehavior{TRequest,TResponse}"/> — role is
/// proven by the role gate, ownership here, work in the handler. Requests that
/// aren't conversation-scoped pass straight through.
/// </summary>
public class ConversationOwnershipBehavior<TRequest, TResponse>(
    ICurrentUser currentUser,
    IMessagingQueries messagingQueries)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMessagingQueries _messagingQueries = messagingQueries;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IOwnedConversationRequest owned)
        {
            var residentId = _currentUser.ResidentId
                ?? throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);

            var ownerResidentId = await _messagingQueries.FindResidentIdAsync(owned.ConversationId, cancellationToken);
            if (ownerResidentId != residentId)
            {
                throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);
            }
        }

        return await next();
    }
}
