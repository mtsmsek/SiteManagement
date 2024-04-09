using FluentValidation;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;

public class UpdateResidentCommandValidator : AbstractValidator<UpdateResidentCommand>
{
    public UpdateResidentCommandValidator()
    {
        var currentTime = DateTime.Now;
        RuleFor(resident => resident.FirstName).NotEmpty().WithMessage("Ad alanı boş bırakılamaz");
        RuleFor(resident => resident.LastName).NotEmpty().WithMessage("Soyad alanı boş bırakılamaz");

        RuleFor(resident => resident.BirthYear).NotEmpty().WithMessage("Doğum yılınız boş bırakılamaz")
            .LessThanOrEqualTo(currentTime.Year).WithMessage("Geçersiz doğum yılı");

        RuleFor(resident => resident.BirthMonth).NotEmpty().WithMessage("Doğum tarihi ay bölümü boş bırakılamaz")
            .Must(MonthValueMustBeBetweenOneAndTwelve).WithMessage("Ay değeri 1 ve 12 arasında olmalı.")
              .Must((command, month) => MonthValueMustBeLessThanOrEqualToCurrentMonthValueWhenBirthYearEqualsToCurrentYear(command.BirthYear, month,                                                                                                                      currentTime))
                                                                                                                        .WithMessage("Ay değeri geçersiz.");
        RuleFor(resident => resident.BirthDay).NotEmpty().WithMessage("Doğum günü bölümü boş bırakılamaz")
            .GreaterThanOrEqualTo(1).LessThanOrEqualTo(31).WithMessage("Geçersiz gün değeri");

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
