namespace SiteManagement.Application.Shared.Validation;

/// <summary>
/// Single source of truth for FluentValidation messages. Used by
/// <see cref="CommonValidationRules"/> and by command-specific validators so
/// the same rule always speaks with the same voice. W1 Day 6 swaps these
/// English defaults for entries in <c>ValidationMessages.tr.resx</c> /
/// <c>.en.resx</c> via <c>IStringLocalizer</c>.
/// </summary>
public static class ValidationMessages
{
    /// <summary>Generic "field is required" message; supply the field name as the format arg.</summary>
    public const string Required = "{PropertyName} is required.";

    /// <summary>"Field must be at most N characters" message; FluentValidation fills the args.</summary>
    public const string MaxLength = "{PropertyName} must be at most {MaxLength} characters.";

    /// <summary>"Field must be at least N characters" message.</summary>
    public const string MinLength = "{PropertyName} must be at least {MinLength} characters.";

    /// <summary>Email format failure.</summary>
    public const string EmailInvalidFormat = "Email format is invalid.";

    /// <summary>Password complexity failure (configured by <see cref="CommonValidationRules.ValidPassword"/>).</summary>
    public const string PasswordTooShort = "Password must be at least {MinLength} characters.";

    /// <summary>Login/refresh failures collapse onto this single message to avoid user enumeration.</summary>
    public const string InvalidCredentials = "Invalid email or password.";

    /// <summary>Refresh token is unknown, expired, or already consumed.</summary>
    public const string InvalidRefreshToken = "Refresh token is invalid or expired.";

    /// <summary>Refresh token's owning user has been removed.</summary>
    public const string RefreshOwnerMissing = "Refresh token owner no longer exists.";
}
