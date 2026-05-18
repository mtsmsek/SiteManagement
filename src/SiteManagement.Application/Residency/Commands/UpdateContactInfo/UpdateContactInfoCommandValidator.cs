using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Residency.Commands.UpdateContactInfo;

/// <summary>Surface-level validation for <see cref="UpdateContactInfoCommand"/>.</summary>
public sealed class UpdateContactInfoCommandValidator : AbstractValidator<UpdateContactInfoCommand>
{
    public UpdateContactInfoCommandValidator()
    {
        RuleFor(x => x.ResidentId).ValidId();
        RuleFor(x => x.Email).ValidEmailAddress();
        RuleFor(x => x.Phone).ValidPhoneNumber();
    }
}
