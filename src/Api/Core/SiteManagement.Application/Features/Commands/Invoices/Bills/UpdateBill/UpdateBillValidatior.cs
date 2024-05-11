using FluentValidation;
using SiteManagement.Application.Validators.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;

public class UpdateBillValidatior : AbstractValidator<UpdateBillCommand>
{
    public UpdateBillValidatior()
    {

        RuleFor(i => i.Fee).ValidateFee();

        RuleFor(i => i.Year).ValidateYear();

        RuleFor(i => i.Month).ValidateMonth();

        RuleFor(i => i.Type).ValidateBillType();
        //TODO write new values cannot be same with old value tests

    }


}
