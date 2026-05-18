namespace SiteManagement.Application.Abstractions.Auth;

/// <summary>
/// Read-only view of the authenticated caller, as visible inside an
/// Application-layer handler. Implementation lives in the Api layer and
/// reads <see cref="System.Security.Claims.ClaimsPrincipal"/> from the
/// current <c>HttpContext</c>. Handlers consume this port — they do not
/// touch ASP.NET Core types directly.
/// </summary>
public interface ICurrentUser
{
    /// <summary>True when an authenticated principal is present on the request.</summary>
    bool IsAuthenticated { get; }

    /// <summary>The user id encoded in the access token's <c>sub</c> claim.</summary>
    /// <remarks>Throws if accessed on an unauthenticated request — guard with <see cref="IsAuthenticated"/>.</remarks>
    Guid UserId { get; }

    /// <summary>
    /// The Domain <c>Resident.Id</c> linked to the caller, when the
    /// caller is a resident. <c>null</c> for admin users.
    /// </summary>
    Guid? ResidentId { get; }

    /// <summary>Convenience predicate: caller has the supplied role.</summary>
    bool IsInRole(string role);
}
