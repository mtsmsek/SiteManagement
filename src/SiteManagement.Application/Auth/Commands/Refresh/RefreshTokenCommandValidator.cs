using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Auth.Commands.Refresh;

/// <summary>Surface-level validation for <see cref="RefreshTokenCommand"/>.</summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).RequiredText();
    }
}
