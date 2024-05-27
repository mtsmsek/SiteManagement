﻿using FluentValidation;
using SiteManagement.Application.Validators.Residents;
using SiteManagement.Domain.Constants.Residents;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;

public class UpdateResidentCommandValidator : AbstractValidator<UpdateResidentCommand>
{
    public UpdateResidentCommandValidator()
    {
        //todo fix the interface for validaton
        var currentTime = DateTime.Now;
        RuleFor(resident => resident.FirstName).ValidateFirstName();
        RuleFor(resident => resident.LastName).ValidateLastName();

        RuleFor(resident => resident.BirthYear).ValidateBirthYear(currentTime);
            

        RuleFor(resident => resident.BirthMonth).ValidateBirthMonth(currentTime);

        RuleFor(resident => resident.BirthDay).ValidateBirthDay(currentTime);

        RuleFor(resident => resident.IdenticalNumber.Length).ValidateIdenticalNumberLength();
            



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
