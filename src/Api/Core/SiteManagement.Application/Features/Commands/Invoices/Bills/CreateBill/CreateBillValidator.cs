using FluentValidation;
using SiteManagement.Domain.Entities.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;

public class CreateBillValidator : AbstractValidator<CreateBillCommand>
{
    public CreateBillValidator()
    {
        //TODO -- fix the messages
        RuleFor(i => i.Fee).GreaterThan(0).WithMessage("fatura ücreti eksi olamaz");

        RuleFor(i => i.Month).NotEmpty().WithMessage("ay alanı boş bırakılamaz")
            .Must(MonthValueMustBeBetweenOneAndTwelve).WithMessage("Ay değeri 1 ve 12 arasında olmalı.")
            .Must(MonthValueMustBeLessThanOrEqualToCurrentMonthValue).WithMessage("Gelecek aylara ait fatura kesemezsiniz.");

        RuleFor(i => i.Year).NotEmpty().WithMessage("yıl alanı boş bırakılamaz")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Gelecek yıla ait fatura kesemezsiniz");

       
    }
    private bool MonthValueMustBeBetweenOneAndTwelve(int month)
    {

        if (month <= 0 && month > 12)
            return false;
        return true;

    }

    private bool MonthValueMustBeLessThanOrEqualToCurrentMonthValue(int month)
    {
        var currentTime = DateTime.Now;
        if (month <= currentTime.Month)
            return true;

        return false;
    }
}
