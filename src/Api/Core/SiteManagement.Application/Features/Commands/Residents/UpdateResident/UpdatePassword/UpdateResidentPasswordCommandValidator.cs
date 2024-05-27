using FluentValidation;
using SiteManagement.Domain.Constants.Residents;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword;

public class UpdateResidentPasswordCommandValidator : AbstractValidator<UpdateResidentPasswordCommand>
{
    private readonly string _specialCharacters = "*-.,<>";
    public UpdateResidentPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.PasswordCannotBeEmpty)
            .Must(PasswordMustBeGreaterThanOrEqualTo8CharactersAndLessThanOrEqualTo16Characters).WithMessage(ResidentMessages.ValidationMessages.PasswordShouldLongerThanEightCharAndLessThanOrEqualToSixTeenChar)
            .Must(PasswordMustContainAtLeast1CapitalLetterAnd1SpecialChracterAnd1Number).WithMessage(ResidentMessages.ValidationMessages.PasswordShouldIncludeAtLeastOneBiggerOneNumberAndSpecialChar);
    }

    private bool PasswordMustBeGreaterThanOrEqualTo8CharactersAndLessThanOrEqualTo16Characters(string password)
        => password.Length >= 8 && password.Length <= 16;
    private bool PasswordMustContainAtLeast1CapitalLetterAnd1SpecialChracterAnd1Number(string password)
        => password.Any(char.IsUpper) && password.Any(char.IsNumber) && password.Any(c => _specialCharacters.Contains(c));
}
