using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.ChangeUtilityBillAmount;

/// <summary>Surface-level validation for <see cref="ChangeUtilityBillAmountCommand"/>.</summary>
public sealed class ChangeUtilityBillAmountCommandValidator : AbstractValidator<ChangeUtilityBillAmountCommand>
{
    public ChangeUtilityBillAmountCommandValidator()
    {
        RuleFor(x => x.UtilityBillPeriodId).ValidId();
        RuleFor(x => x.TotalAmount).PositiveAmount();
    }
}
