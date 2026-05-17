using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Auth.Commands.Login;

/// <summary>Surface-level validation for <see cref="LoginCommand"/>.</summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).ValidEmailAddress();
        RuleFor(x => x.Password).RequiredText();
    }
}
