using FluentValidation;

namespace SiteManagement.Application.Shared.Validation;

/// <summary>
/// Reusable FluentValidation rules so the same constraint always emits the
/// same message. Validators should prefer these over inline rule chains.
/// </summary>
public static class CommonValidationRules
{
    /// <summary>
    /// Required, well-formed email address. Shared by every command that
    /// accepts an email so the message stays consistent.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidEmailAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .EmailAddress().WithMessage(ValidationMessages.EmailInvalidFormat);

    /// <summary>
    /// Password is required and meets the minimum length defined by
    /// <see cref="ValidationLimits.PasswordMinLength"/>. Complexity rules
    /// (digit/uppercase/special) are enforced by Identity Options at the
    /// infrastructure layer to keep the application rule simple.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MinimumLength(ValidationLimits.PasswordMinLength).WithMessage(ValidationMessages.PasswordTooShort);

    /// <summary>
    /// Required full name within <see cref="ValidationLimits.FullNameMaxLength"/>.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidFullName<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(ValidationLimits.FullNameMaxLength).WithMessage(ValidationMessages.MaxLength);

    /// <summary>
    /// Required non-empty string (no length cap). Useful for opaque tokens
    /// such as a refresh token.
    /// </summary>
    public static IRuleBuilderOptions<T, string> RequiredText<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.NotEmpty().WithMessage(ValidationMessages.Required);
}
