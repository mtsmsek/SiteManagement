using FluentValidation;
using SiteManagement.Application.Shared.Validation;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.CreateSite;

/// <summary>Surface-level validation for <see cref="CreateSiteCommand"/>.</summary>
public sealed class CreateSiteCommandValidator : AbstractValidator<CreateSiteCommand>
{
    public CreateSiteCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(PropertyLimits.SiteNameMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(PropertyLimits.SiteAddressMaxLength).WithMessage(ValidationMessages.MaxLength);
    }
}
