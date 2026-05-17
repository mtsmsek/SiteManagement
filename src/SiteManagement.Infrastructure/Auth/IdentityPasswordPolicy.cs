namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Identity password complexity numbers in one place. Mirrors what is shown
/// to the user via FluentValidation so the two stay in sync.
/// </summary>
public static class IdentityPasswordPolicy
{
    /// <summary>Minimum password length enforced by Identity.</summary>
    public const int RequiredLength = 8;

    /// <summary>Require at least one digit?</summary>
    public const bool RequireDigit = true;

    /// <summary>Require at least one lowercase letter?</summary>
    public const bool RequireLowercase = true;

    /// <summary>Require at least one uppercase letter?</summary>
    public const bool RequireUppercase = true;

    /// <summary>Require at least one non-alphanumeric character?</summary>
    public const bool RequireNonAlphanumeric = false;
}
