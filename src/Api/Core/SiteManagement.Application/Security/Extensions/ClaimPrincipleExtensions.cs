using System.Security.Claims;

namespace SiteManagement.Application.Security.Extensions;

public static class ClaimPrincipleExtensions
{
    public static List<string>? Claims(this ClaimsPrincipal claimPrincipal, string claimType)
    {
        var result = claimPrincipal?.FindAll(claimType)?.Select(x => x.Value).ToList();

        return result;
    }

    public static List<string>? ClaimRoles(this ClaimsPrincipal claimPrincipal)
        => claimPrincipal?.Claims(ClaimTypes.Role);

    public static Guid GetUserId(this ClaimsPrincipal claimPrincipal)
        => new(claimPrincipal?.Claims(ClaimTypes.NameIdentifier)?.FirstOrDefault()!);
}
