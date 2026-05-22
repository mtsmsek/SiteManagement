using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.MarkUtilityBillItemPaid;

/// <summary>Surface-level validation for <see cref="MarkUtilityBillItemPaidCommand"/>.</summary>
public sealed class MarkUtilityBillItemPaidCommandValidator : AbstractValidator<MarkUtilityBillItemPaidCommand>
{
    public MarkUtilityBillItemPaidCommandValidator()
    {
        RuleFor(x => x.UtilityBillPeriodId).ValidId();
        RuleFor(x => x.ItemId).ValidId();
    }
}
