using FluentValidation;

namespace SiteManagement.Application.Features.Commands.Residents.CreateResident;

public class CreateResidentValidator : AbstractValidator<CreateResidentCommand>
{
    public CreateResidentValidator()
    {
        var currentTime = DateTime.Now;
        RuleFor(resident => resident.FirstName).NotEmpty().WithMessage("Ad alanı boş bırakılamaz");
        RuleFor(resident => resident.LastName).NotEmpty().WithMessage("Soyad alanı boş bırakılamaz");

        RuleFor(resident => resident.BirthYear).NotEmpty().WithMessage("Doğum yılınız boş bırakılamaz")
            .LessThanOrEqualTo(currentTime.Year).WithMessage("Geçersiz doğum yılı");

        RuleFor(resident => resident.BirthMonth).NotEmpty().WithMessage("Doğum tarihi ay bölümü boş bırakılamaz")
            .Must(MonthValueMustBeBetweenOneAndTwelve).WithMessage("Ay değeri 1 ve 12 arasında olmalı.");


        RuleFor(resident => resident.BirthMonth).NotEmpty().WithMessage("Doğum tarihi ay bölümü boş bırakılamaz")
                .Must((command, month) => MonthValueMustBeLessThanOrEqualToCurrentMonthValueWhenBirthYearEqualsToCurrentYear(command.BirthYear, month, currentTime)).WithMessage("Ay değeri geçersiz.");


        RuleFor(resident => resident.Email).NotEmpty().WithMessage("Email alanı boş bırakılamaz")
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
            .WithMessage("Geçersiz email");

        RuleFor(resident => resident.IdenticalNumber.Length)
            .Equal(11).WithMessage("Kimlik numaranız 11 haneden oluşmalıdır");



    }
    private bool MonthValueMustBeLessThanOrEqualToCurrentMonthValueWhenBirthYearEqualsToCurrentYear(int birthYear, int birthMonth, DateTime currentTime)
    {
        if (birthYear == currentTime.Year)
        {
            return birthMonth <= currentTime.Month;
        }

        return true;
    }
    private bool MonthValueMustBeBetweenOneAndTwelve(int month)
    {

        if (month <= 0 && month > 12)
            return false;
        return true;

    }
}
