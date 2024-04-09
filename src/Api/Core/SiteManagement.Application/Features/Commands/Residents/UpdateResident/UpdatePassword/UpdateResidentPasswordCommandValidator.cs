using FluentValidation;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword;

public class UpdateResidentPasswordCommandValidator : AbstractValidator<UpdateResidentPasswordCommand>
{
    private readonly string _specialCharacters = "*-.,<>";
    public UpdateResidentPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty()
            .Must(PasswordMustBeGreaterThanOrEqualTo8CharactersAndLessThanOrEqualTo16Characters).WithMessage("Paralonuz 8 karakterden uzun olmalıdır.")
            .Must(PasswordMustContainAtLeast1CapitalLetterAnd1SpecialChracterAnd1Number).WithMessage("Parolanız en az bir büyük harf, bir sayı ve bir özel karakter içermelidir");
    }

    private bool PasswordMustBeGreaterThanOrEqualTo8CharactersAndLessThanOrEqualTo16Characters(string password)
        => password.Length >= 8 && password.Length <= 16;
    private bool PasswordMustContainAtLeast1CapitalLetterAnd1SpecialChracterAnd1Number(string password)
        => password.Any(char.IsUpper) && password.Any(char.IsNumber) && password.Any(c => _specialCharacters.Contains(c));
}
