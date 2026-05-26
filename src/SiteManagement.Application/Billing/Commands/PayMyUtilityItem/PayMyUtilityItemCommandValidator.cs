using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.PayMyUtilityItem;

/// <summary>
/// Surface-level validation for <see cref="PayMyUtilityItemCommand"/>. Shares the
/// card rules with every other pay path; ownership is the pipeline's job.
/// </summary>
public sealed class PayMyUtilityItemCommandValidator : AbstractValidator<PayMyUtilityItemCommand>
{
    public PayMyUtilityItemCommandValidator()
    {
        RuleFor(x => x.UtilityBillPeriodId).ValidId();
        RuleFor(x => x.ItemId).ValidId();
        RuleFor(x => x.CardNumber).ValidCardNumber();
        RuleFor(x => x.Cvv).ValidCvv();
    }
}
