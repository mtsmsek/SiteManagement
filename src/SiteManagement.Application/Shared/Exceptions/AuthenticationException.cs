namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown by auth handlers when credentials are wrong, refresh tokens expired,
/// or a user is locked out. Renders as HTTP 401.
/// </summary>
public sealed class AuthenticationException : ApplicationException
{
    /// <summary>Creates a 401 with a localized message.</summary>
    public AuthenticationException(string message) : base(message, HttpStatus.Unauthorized) { }
}
