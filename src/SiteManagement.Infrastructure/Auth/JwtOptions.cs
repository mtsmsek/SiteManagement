namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Strongly-typed settings for JWT issuance, bound to the <c>Jwt</c>
/// configuration section. The default <see cref="AccessTokenMinutes"/> and
/// <see cref="RefreshTokenDays"/> match the values in <c>.env.example</c>.
/// </summary>
public class JwtOptions
{
    /// <summary>Configuration section name bound at startup.</summary>
    public const string SectionName = "Jwt";

    /// <summary>Symmetric signing key (HS256). Must be at least 32 characters.</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>Issuer claim (<c>iss</c>) embedded into every access token.</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>Audience claim (<c>aud</c>) embedded into every access token.</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Access-token lifetime in minutes.</summary>
    public int AccessTokenMinutes { get; init; } = 60;

    /// <summary>Refresh-token lifetime in days.</summary>
    public int RefreshTokenDays { get; init; } = 14;
}
