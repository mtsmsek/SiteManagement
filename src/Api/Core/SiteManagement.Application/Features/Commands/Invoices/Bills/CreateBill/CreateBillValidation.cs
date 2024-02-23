using FluentValidation;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;

public class CreateBillValidation : AbstractValidator<Bill>
{
    public CreateBillValidation()
    {
        //TODO -- fix the messages
        RuleFor(i => i.Fee).GreaterThan(0).WithMessage("fatura ücreti eksi olamaz");
        RuleFor(i => i.Year).NotEmpty().WithMessage("yıl alanı boş bırakılamaz");
        RuleFor(i => i.Month).NotEmpty().WithMessage("ay alanı boş bırakılamaz");
    }

    
}
