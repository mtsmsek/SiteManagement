namespace SiteManagement.Application.Abstractions.Auth;

/// <summary>
/// Application-facing port for issuing access &amp; refresh tokens. The
/// Application layer never references <c>JwtSecurityTokenHandler</c> directly
/// — the Infrastructure layer supplies the concrete implementation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Issues a fresh access + refresh token pair for the given user.
    /// </summary>
    /// <param name="userId">Identifier of the authenticated user.</param>
    /// <param name="email">Email claim embedded in the access token.</param>
    /// <param name="roles">Roles to encode as <c>ClaimTypes.Role</c> claims.</param>
    AuthTokens IssueTokens(Guid userId, string email, IEnumerable<string> roles);
}

/// <summary>
/// Result of <see cref="ITokenService.IssueTokens"/>: a signed access JWT and
/// an opaque refresh token, each with its expiry stamp in UTC.
/// </summary>
public sealed record AuthTokens(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
