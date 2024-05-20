using FluentValidation;
using SiteManagement.Domain.Constants.Residents;

namespace SiteManagement.Application.Features.Commands.Residents.CreateResident;

public class CreateResidentValidator : AbstractValidator<CreateResidentCommand>
{
    public CreateResidentValidator()
    {

        var currentTime = DateTime.Now;
        RuleFor(resident => resident.FirstName).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.FirstNameCannotBeEmpty);
        RuleFor(resident => resident.LastName).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.LastNameCannotBeEmpty);

        RuleFor(resident => resident.BirthYear).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.BirthYearCannotBeEmpty)
            .LessThanOrEqualTo(currentTime.Year).WithMessage(ResidentMessages.ValidationMessages.InvalidBirthYear);

        RuleFor(resident => resident.BirthMonth).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.BirthMonthCannotBeEmpty)
            .Must(MonthValueMustBeBetweenOneAndTwelve)
            .WithMessage(ResidentMessages.ValidationMessages.MonthValueMustBeBetweenOneAndTwelve)
            .Must((command, month) => MonthValueMustBeLessThanOrEqualToCurrentMonthValueWhenBirthYearEqualsToCurrentYear(command.BirthYear, month, currentTime)).WithMessage(ResidentMessages.ValidationMessages.InvalidBirthMonth);

        RuleFor(resident => resident.BirthDay).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.BirthDayCannotBeEmpty)
             .DependentRules(() =>
             {
                     RuleFor(resident => resident.BirthDay)
                     .Must((resident, birthDay) => BirthMustBeLessThanOrEqualToCurrentTime(DateTime.Now, resident.BirthYear, resident.BirthMonth, birthDay))
                     .WithMessage(ResidentMessages.ValidationMessages.InvalidBirthDay);
             });


        RuleFor(resident => resident.Email).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.EmailCannotBeEmpty)
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
            .WithMessage(ResidentMessages.ValidationMessages.InvalidEmail);

        RuleFor(resident => resident.IdenticalNumber.Length)
            .Equal(11).WithMessage(ResidentMessages.ValidationMessages.IdenticalNumberMustIncludeElevenChar);

        RuleFor(resident => resident.PhoneNumber).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.PhoneNumberCannotBeEmpty)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage(ResidentMessages.ValidationMessages.InvalidPhoneNumber);



    }
    private bool BirthMustBeLessThanOrEqualToCurrentTime(DateTime currentTime,int year,int month,int day)
    {
       var maxDay = DateTime.DaysInMonth(year,month);
        if(day > maxDay)
            return false;

        var birthDay = new DateTime(year, month, day);
        if (birthDay > currentTime)
            return false;

        return true;

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

        if (month <= 0 || month > 12)
            return false;
        return true;

    }
}
