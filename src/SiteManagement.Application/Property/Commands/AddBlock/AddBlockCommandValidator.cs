using FluentValidation;
using SiteManagement.Application.Shared.Validation;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.AddBlock;

/// <summary>Surface-level validation for <see cref="AddBlockCommand"/>.</summary>
public sealed class AddBlockCommandValidator : AbstractValidator<AddBlockCommand>
{
    public AddBlockCommandValidator()
    {
        RuleFor(x => x.SiteId).ValidId();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(PropertyLimits.BlockNameMaxLength).WithMessage(ValidationMessages.MaxLength);
    }
}
