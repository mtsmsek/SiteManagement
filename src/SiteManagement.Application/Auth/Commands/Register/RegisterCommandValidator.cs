using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Auth.Commands.Register;

/// <summary>Validation rules for <see cref="RegisterCommand"/>.</summary>
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).ValidEmailAddress();
        RuleFor(x => x.Password).ValidPassword();
        RuleFor(x => x.FullName).ValidFullName();
    }
}
