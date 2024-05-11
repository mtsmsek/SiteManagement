using FluentValidation;
using SiteManagement.Application.Validators.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;

public class CreateBillValidator : AbstractValidator<CreateBillCommand>
{
    public CreateBillValidator()
    {
        RuleFor(i => i.Fee).ValidateFee();

        RuleFor(i => i.Year).ValidateYear();
 
        RuleFor(i => i.Month).ValidateMonth();

        RuleFor(i => i.Type).ValidateBillType();



    }

}
