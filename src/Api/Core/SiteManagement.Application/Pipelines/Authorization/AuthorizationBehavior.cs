using MediatR;
using Microsoft.AspNetCore.Http;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Security.Constants;
using SiteManagement.Application.Security.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace SiteManagement.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<string>? userRolesClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();

        if (userRolesClaims == null)
            throw new AuthorizationException("Claims not found.");


        bool isNotMatchedAUserRoleClaimWithRequestRoles = userRolesClaims
            .FirstOrDefault(userRolesClaim => userRolesClaim == GeneralOperationClaims.Admin ||
                                                               request.Roles.Any(role => role == userRolesClaim))
                                                               .IsNullOrEmpty();
        if(isNotMatchedAUserRoleClaimWithRequestRoles)
            throw new AuthorizationException("You are not authorized!");


        TResponse response  = await next();
        return response;
    }
}
