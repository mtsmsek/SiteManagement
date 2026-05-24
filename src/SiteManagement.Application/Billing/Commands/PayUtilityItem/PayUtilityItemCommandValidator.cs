using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.PayUtilityItem;

/// <summary>
/// Surface-level validation for <see cref="PayUtilityItemCommand"/>. Deep card
/// validity (Luhn, CVV shape, expiry) is the payment gateway's job; here we
/// only check the ids and that card fields are present.
/// </summary>
public sealed class PayUtilityItemCommandValidator : AbstractValidator<PayUtilityItemCommand>
{
    public PayUtilityItemCommandValidator()
    {
        RuleFor(x => x.UtilityBillPeriodId).ValidId();
        RuleFor(x => x.ItemId).ValidId();
        RuleFor(x => x.CardNumber).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Cvv).NotEmpty().WithMessage(ValidationMessages.Required);
    }
}
