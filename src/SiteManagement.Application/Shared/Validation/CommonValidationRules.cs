using FluentValidation;
using SiteManagement.Domain.Residency;

namespace SiteManagement.Application.Shared.Validation;

/// <summary>
/// Reusable FluentValidation rules. Validators emit <see cref="ValidationMessages"/>
/// keys as the message text — the API layer's exception middleware looks
/// each key up via <c>IStringLocalizer&lt;ValidationMessages&gt;</c> when
/// rendering the response, so validators stay free of any localization or
/// framework reference.
/// </summary>
/// <remarks>
/// These rules carry only the surface-level shape (required / length /
/// rough format). Deep validity — TcNo checksum, plate province code,
/// email-shape regex — lives on the value objects in the Domain layer and
/// surfaces as <c>DomainException</c>s the pipeline turns into 409s.
/// Validators here exist to fail fast with a 400 before the handler
/// reaches the domain.
/// </remarks>
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
    /// <see cref="ValidationLimits.PasswordMinLength"/>.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MinimumLength(ValidationLimits.PasswordMinLength).WithMessage(ValidationMessages.PasswordTooShort);

    /// <summary>
    /// Required full name within <see cref="ValidationLimits.FullNameMaxLength"/>.
    /// Used by single-string full-name fields (e.g. AppUser registration).
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidFullName<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(ValidationLimits.FullNameMaxLength).WithMessage(ValidationMessages.MaxLength);

    /// <summary>
    /// One component of a person's name (first or last). Required, capped
    /// at <see cref="ResidencyLimits.NameComponentMaxLength"/>. Used by
    /// resident commands that take FirstName + LastName separately so the
    /// FullName value object can compose them.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidNameComponent<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(ResidencyLimits.NameComponentMaxLength).WithMessage(ValidationMessages.MaxLength);

    /// <summary>
    /// Required non-empty string (no length cap). Useful for opaque tokens
    /// such as a refresh token.
    /// </summary>
    public static IRuleBuilderOptions<T, string> RequiredText<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.NotEmpty().WithMessage(ValidationMessages.Required);

    /// <summary>
    /// Required non-empty <see cref="Guid"/> id (rejects <c>Guid.Empty</c>).
    /// Used everywhere a command references another aggregate by id — site,
    /// block, apartment, resident — so the message is identical across the API.
    /// </summary>
    public static IRuleBuilderOptions<T, Guid> ValidId<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        => ruleBuilder.NotEmpty().WithMessage(ValidationMessages.Required);

    /// <summary>
    /// Required non-empty nullable <see cref="Guid"/> id. Same rule as
    /// <see cref="ValidId{T}"/> but for <c>Guid?</c> properties.
    /// </summary>
    public static IRuleBuilderOptions<T, Guid?> ValidId<T>(this IRuleBuilder<T, Guid?> ruleBuilder)
        => ruleBuilder.NotEmpty().WithMessage(ValidationMessages.Required);

    /// <summary>
    /// Inclusive numeric range with the standard <see cref="ValidationMessages.Required"/>
    /// message. Removes the boilerplate of repeating <c>.InclusiveBetween(min, max).WithMessage(...)</c>
    /// across every numeric command field.
    /// </summary>
    public static IRuleBuilderOptions<T, int> InRange<T>(this IRuleBuilder<T, int> ruleBuilder, int min, int max)
        => ruleBuilder.InclusiveBetween(min, max).WithMessage(ValidationMessages.Required);

    /// <summary>
    /// A monetary amount that must be strictly positive. Shared by every
    /// billing command that takes an amount (dues, utility totals) so the
    /// "greater than zero" rule and its message live in one place rather than
    /// repeating <c>.GreaterThan(0)</c> with a magic literal across validators.
    /// </summary>
    public static IRuleBuilderOptions<T, decimal> PositiveAmount<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        => ruleBuilder.GreaterThan(decimal.Zero).WithMessage(ValidationMessages.AmountNotPositive);

    /// <summary>
    /// Required Turkish citizenship number — surface-level check only
    /// (non-empty, exactly <see cref="ResidencyLimits.TcNoLength"/> digits).
    /// The checksum validation lives on the <c>TcNo</c> value object.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidTcNo<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Length(ResidencyLimits.TcNoLength, ResidencyLimits.TcNoLength)
            .WithMessage(ValidationMessages.Required);

    /// <summary>
    /// Required Turkish phone number — surface-level check only
    /// (non-empty, length-capped). Full format parsing lives on the
    /// <c>PhoneNumber</c> value object.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(ResidencyLimits.PhoneNumberMaxLength).WithMessage(ValidationMessages.MaxLength);

    /// <summary>
    /// Required Turkish license plate — surface-level check only.
    /// Format + province-code validation lives on the <c>PlateNumber</c>
    /// value object.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidPlateNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(ResidencyLimits.PlateNumberMaxLength).WithMessage(ValidationMessages.MaxLength);
}
