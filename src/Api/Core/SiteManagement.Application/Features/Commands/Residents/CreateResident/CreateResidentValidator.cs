using FluentValidation;
using SiteManagement.Application.Validators.Residents;
using SiteManagement.Domain.Constants.Residents;

namespace SiteManagement.Application.Features.Commands.Residents.CreateResident;

public class CreateResidentValidator : AbstractValidator<CreateResidentCommand>
{
    public CreateResidentValidator()
    {

        var currentTime = DateTime.Now;

        RuleFor(resident => resident.FirstName).ValidateFirstName();
        RuleFor(resident => resident.LastName).ValidateLastName();
        RuleFor(resident => resident.BirthYear).ValidateBirthYear(currentTime);


        RuleFor(resident => resident.BirthMonth).ValidateBirthMonth(currentTime);
        RuleFor(resident => resident.BirthDay).ValidateBirthDay(currentTime);

        RuleFor(resident => resident.Email).ValidateEmail();
        RuleFor(resident => resident.IdenticalNumber.Length).ValidateIdenticalNumberLength();
        RuleFor(resident => resident.PhoneNumber).ValidatePhoneNumber();
        
        









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
