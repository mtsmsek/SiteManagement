using FluentValidation;
using SiteManagement.Domain.Constants.Residents;

namespace SiteManagement.Application.Validators.Residents;

public static class BaseResidentValidatorExtensions
{
    public static void ValidateFirstName<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand, string> ruleBuilder)
        where TResidentCommand : IResidentCommand
    {
        ruleBuilder.NotEmpty().WithMessage(ResidentMessages.ValidationMessages.FirstNameCannotBeEmpty);
    }
    public static void ValidateLastName<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand, string> ruleBuilder)
        where TResidentCommand : IResidentCommand
    {
        ruleBuilder.NotEmpty().WithMessage(ResidentMessages.ValidationMessages.LastNameCannotBeEmpty);

    }
    public static void ValidateBirthYear<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand, int> ruleBuilder, DateTime currentTime)
        where TResidentCommand : IResidentCommand
    {

        ruleBuilder.NotEmpty().WithMessage(ResidentMessages.ValidationMessages.BirthYearCannotBeEmpty)
        .LessThanOrEqualTo(currentTime.Year).WithMessage(ResidentMessages.ValidationMessages.InvalidBirthYear);
    }
    public static void ValidateBirthMonth<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand, int> ruleBuilder, DateTime currentTime)
        where TResidentCommand : IResidentCommand
    {
        ruleBuilder.NotEmpty().WithMessage(ResidentMessages.ValidationMessages.BirthMonthCannotBeEmpty)
       .Must(MonthValueMustBeBetweenOneAndTwelve)
       .WithMessage(ResidentMessages.ValidationMessages.MonthValueMustBeBetweenOneAndTwelve)
       .Must((command, month) => MonthValueMustBeLessThanOrEqualToCurrentMonthValueWhenBirthYearEqualsToCurrentYear(command.BirthYear, month, currentTime)).WithMessage(ResidentMessages.ValidationMessages.InvalidBirthMonth);
    }
    public static void ValidateBirthDay<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand, int> ruleBuilder, DateTime currentTime)
        where TResidentCommand : IResidentCommand
    {
        ruleBuilder.NotEmpty().WithMessage(ResidentMessages.ValidationMessages.BirthDayCannotBeEmpty)
     .DependentRules(() =>
     {
         ruleBuilder
                 .Must((resident, birthDay) => BirthMustBeLessThanOrEqualToCurrentTime(currentTime, resident.BirthYear, resident.BirthMonth, birthDay))
                 .WithMessage(ResidentMessages.ValidationMessages.InvalidBirthDay);
     });
    }

    public static void ValidateEmail<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand, string> ruleBuilder)
        where TResidentCommand : IResidentCommand
    {
        ruleBuilder.NotEmpty().WithMessage(ResidentMessages.ValidationMessages.EmailCannotBeEmpty)
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
            .WithMessage(ResidentMessages.ValidationMessages.InvalidEmail);
    }
    public static void ValidateIdenticalNumberLength<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand, int> ruleBuilder)
        where TResidentCommand : IResidentCommand
    {
        ruleBuilder
            .Equal(11).WithMessage(ResidentMessages.ValidationMessages.IdenticalNumberMustIncludeElevenChar);
    }

    public static void ValidatePhoneNumber<TResidentCommand>(this IRuleBuilderInitial<TResidentCommand,string> ruleBuilder)
        where TResidentCommand : IResidentCommand
    {

        ruleBuilder.NotEmpty().WithMessage(ResidentMessages.ValidationMessages.PhoneNumberCannotBeEmpty)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage(ResidentMessages.ValidationMessages.InvalidPhoneNumber);
    }
    private static bool BirthMustBeLessThanOrEqualToCurrentTime(DateTime currentTime, int year, int month, int day)
    {
        if (day == 0)
            return false;

        var maxDay = DateTime.DaysInMonth(year, month);
        if (day > maxDay)
            return false;

        var birthDay = new DateTime(year, month, day);
        if (birthDay > currentTime)
            return false;

        return true;

    }
    private static bool MonthValueMustBeLessThanOrEqualToCurrentMonthValueWhenBirthYearEqualsToCurrentYear(int birthYear, int birthMonth, DateTime currentTime)
    {
        if (birthYear == currentTime.Year)
        {
            return birthMonth < currentTime.Month;
        }

        return true;
    }
    private static bool MonthValueMustBeBetweenOneAndTwelve(int month)
    {

        if (month <= 0 || month > 12)
            return false;
        return true;

    }
}
