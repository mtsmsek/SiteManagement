namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown by auth handlers when credentials are wrong, refresh tokens expired,
/// or a user is locked out. Renders as HTTP 401. Carries the stable
/// <see cref="MessageKey"/> so the API middleware can render a localized
/// detail message based on the active request culture.
/// </summary>
public sealed class AuthenticationException : ApplicationException
{
    /// <summary>Creates a 401 carrying a stable resource key.</summary>
    /// <param name="messageKey">Resource key used for both <see cref="Exception.Message"/> and the localized lookup.</param>
    public AuthenticationException(string messageKey) : base(messageKey, HttpStatus.Unauthorized)
    {
        MessageKey = messageKey;
    }

    /// <summary>Stable resource key (e.g. <c>"Auth.InvalidCredentials"</c>).</summary>
    public string MessageKey { get; }
}
