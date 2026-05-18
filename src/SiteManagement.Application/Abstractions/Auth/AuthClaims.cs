namespace SiteManagement.Application.Abstractions.Auth;

/// <summary>
/// Custom JWT claim names emitted by <see cref="ITokenService"/> and read
/// back through <see cref="ICurrentUser"/>. Standardised here so the
/// token-emit side and the read side stay perfectly aligned.
/// </summary>
public static class AuthClaims
{
    /// <summary>Carries the linked <c>Resident.Id</c> (only present for resident users).</summary>
    public const string ResidentId = "resident_id";
}
