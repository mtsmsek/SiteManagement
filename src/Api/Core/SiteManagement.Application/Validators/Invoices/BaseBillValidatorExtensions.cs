using FluentValidation;
using SiteManagement.Domain.Constants.Invoices.Bills;
using SiteManagement.Domain.Enumarations.Invoices;

namespace SiteManagement.Application.Validators.Invoices;

public static class BaseBillValidatorExtensions
{
    public static void ValidateFee<TBillCommand>(this IRuleBuilderInitial<TBillCommand, double> ruleBuilder)
        where TBillCommand : IBillCommandToValidate
    {
        ruleBuilder.GreaterThan(0).WithMessage(BillsMessages.ValidationMessages.BillFeeCannotBeLessThanOrEqualToZero);
    }
    public static void ValidateMonth<TBillCommand>(this IRuleBuilderInitial<TBillCommand, int> ruleBuilder)
        where TBillCommand : IBillCommandToValidate

    {

        ruleBuilder.NotEmpty().WithMessage(BillsMessages.ValidationMessages.MonthCannotBeNull)
            .Must(MonthValueMustBeBetweenOneAndTwelve).WithMessage(BillsMessages.ValidationMessages.MonthValueShouldBeBetweenOneAndTwelve)
            .Must(MonthValueMustBeLessThanOrEqualToCurrentMonthValue).WithMessage(BillsMessages.ValidationMessages.MonthValueShouldBeLessThanOrEqualToCurrentMonthValue);
    }

    public static void ValidateYear<TBillCommand>(this IRuleBuilderInitial<TBillCommand, int> ruleBuilder)
        where TBillCommand : IBillCommandToValidate

    {
        ruleBuilder.NotEmpty().WithMessage(BillsMessages.ValidationMessages.YearCannotBeNull)
                                .LessThanOrEqualTo(DateTime.Now.Year)
                                .WithMessage(BillsMessages.ValidationMessages.YearValueShouldBeLessThanOrEqualToCurrentYearValue);
    }
    public static void ValidateBillType<TBillCommand>(this IRuleBuilderInitial<TBillCommand, int> ruleBuilder)
        where TBillCommand : IBillCommandToValidate
    {
            ruleBuilder.NotEmpty().WithMessage(BillsMessages.ValidationMessages.BillTypeCannotBeNull)
           .Must(BillTypeShouldBeExist).WithMessage(BillsMessages.ValidationMessages.InvalidBillType);
    }



    private static bool MonthValueMustBeBetweenOneAndTwelve(int month)
    {

        if (month <= 0 || month > 12)
            return false;
        return true;

    }

    private static bool MonthValueMustBeLessThanOrEqualToCurrentMonthValue(int month)
    {
        var currentTime = DateTime.Now;
        if (month <= currentTime.Month)
            return true;

        return false;
    }
    private static bool BillTypeShouldBeExist(int type)
    {
        return BillType.Enumarations.Keys.Contains(type);
    }
}
