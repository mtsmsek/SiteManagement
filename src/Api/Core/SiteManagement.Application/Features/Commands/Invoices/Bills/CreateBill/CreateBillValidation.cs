using FluentValidation;
using SiteManagement.Domain.Entities.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;

public class CreateBillValidation : AbstractValidator<Bill>
{
    public CreateBillValidation()
    {
        //TODO -- fix the messages
        RuleFor(i => i.Fee).GreaterThan(0).WithMessage("fatura ücreti eksi olamaz");
        RuleFor(i => i.Year).NotEmpty().WithMessage("yıl alanı boş bırakılamaz");
        RuleFor(i => i.Month).NotEmpty().WithMessage("ay alanı boş bırakılamaz");
        RuleFor(i => i.Month.Value).LessThanOrEqualTo(DateTime.Now.Month).WithMessage("Gelecek döneme ait fatura kesemezsiniz");
        RuleFor(i => i.Year).LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Gelecek yıla ait fatura kesemezsiniz");


    }

    
}
