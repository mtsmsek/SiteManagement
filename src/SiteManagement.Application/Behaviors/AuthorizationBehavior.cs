using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Application.Behaviors;

/// <summary>
/// Enforces each request's declared authorization requirement
/// (<see cref="IAdminRequest"/> / <see cref="IResidentRequest"/> /
/// <see cref="IPublicRequest"/>) from a single place, so handlers and
/// controllers carry no role logic at all. Runs early in the pipeline (right
/// after logging, before validation) and <strong>fails closed</strong>: a
/// request that declares no requirement is denied — an architecture test makes
/// that omission a build error too.
/// </summary>
/// <remarks>
/// Authentication (401) is the HTTP layer's job (<c>[Authorize]</c>); by the
/// time a request reaches this behavior the caller is authenticated, so this
/// only decides role / scope access and therefore renders as 403. Resource
/// <em>ownership</em> ("is this <em>my</em> bill?") stays in the handler: it
/// needs data the pipeline can't see, so the behavior proves the role and the
/// handler proves the ownership.
/// </remarks>
public class AuthorizationBehavior<TRequest, TResponse>(ICurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUser _currentUser = currentUser;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        switch (request)
        {
            case IPublicRequest:
                break;
            case IAdminRequest:
                Require(_currentUser.IsInRole(Roles.Admin));
                break;
            case IResidentRequest:
                Require(_currentUser.IsInRole(Roles.Resident) && _currentUser.ResidentId is not null);
                break;
            default:
                // Fail closed: an unmarked request is never implicitly allowed.
                throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);
        }

        return await next();
    }

    private static void Require(bool allowed)
    {
        if (!allowed)
        {
            throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);
        }
    }
}
