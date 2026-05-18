using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SiteManagement.Application.Abstractions.Auth;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// HttpContext-backed implementation of <see cref="ICurrentUser"/>. Reads
/// the <see cref="ClaimsPrincipal"/> attached by the JWT bearer middleware
/// and surfaces the bits handlers actually care about. Registered as a
/// scoped service so each request gets a fresh instance.
/// </summary>
public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private const string UnauthenticatedAccessMessage =
        "Current request has no authenticated user.";

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    /// <inheritdoc />
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public Guid UserId
    {
        get
        {
            var sub = Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                   ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new InvalidOperationException(UnauthenticatedAccessMessage);

            return Guid.Parse(sub);
        }
    }

    /// <inheritdoc />
    public Guid? ResidentId
    {
        get
        {
            var raw = Principal?.FindFirstValue(AuthClaims.ResidentId);
            return Guid.TryParse(raw, out var rid) ? rid : null;
        }
    }

    /// <inheritdoc />
    public bool IsInRole(string role) => Principal?.IsInRole(role) ?? false;
}
