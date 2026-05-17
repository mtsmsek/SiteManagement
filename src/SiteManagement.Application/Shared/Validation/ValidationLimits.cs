namespace SiteManagement.Application.Shared.Validation;

/// <summary>
/// Numeric limits used by <see cref="CommonValidationRules"/>. Keeping them
/// here avoids magic numbers scattered across validators and lets
/// documentation point to a single source of truth.
/// </summary>
public static class ValidationLimits
{
    /// <summary>Minimum length of a user-chosen password.</summary>
    public const int PasswordMinLength = 8;

    /// <summary>Maximum length stored for a user's full name.</summary>
    public const int FullNameMaxLength = 120;
}
