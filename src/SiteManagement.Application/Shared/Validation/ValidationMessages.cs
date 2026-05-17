namespace SiteManagement.Application.Shared.Validation;

/// <summary>
/// Stable resource keys consumed by FluentValidation rules. Validators emit
/// these keys as message text; the API layer's exception middleware resolves
/// them to localized strings via <c>IStringLocalizer&lt;ValidationMessages&gt;</c>.
/// </summary>
public sealed class ValidationMessages
{
    /// <summary>"{PropertyName} is required."</summary>
    public const string Required = "Validation.Required";

    /// <summary>"{PropertyName} must be at most {MaxLength} characters."</summary>
    public const string MaxLength = "Validation.MaxLength";

    /// <summary>"{PropertyName} must be at least {MinLength} characters."</summary>
    public const string MinLength = "Validation.MinLength";

    /// <summary>"Email format is invalid."</summary>
    public const string EmailInvalidFormat = "Validation.EmailInvalidFormat";

    /// <summary>"Password must be at least {MinLength} characters." (password-specific copy)</summary>
    public const string PasswordTooShort = "Validation.PasswordTooShort";
}
