using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Residency.Commands.RegisterResident;

/// <summary>Surface-level validation for <see cref="RegisterResidentCommand"/>.</summary>
public sealed class RegisterResidentCommandValidator : AbstractValidator<RegisterResidentCommand>
{
    public RegisterResidentCommandValidator()
    {
        RuleFor(x => x.TcNo).ValidTcNo();
        RuleFor(x => x.FirstName).ValidNameComponent();
        RuleFor(x => x.LastName).ValidNameComponent();
        RuleFor(x => x.Email).ValidEmailAddress();
        RuleFor(x => x.Phone).ValidPhoneNumber();
    }
}
