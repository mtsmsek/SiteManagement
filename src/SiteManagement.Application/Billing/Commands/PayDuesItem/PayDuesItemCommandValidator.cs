using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.PayDuesItem;

/// <summary>
/// Surface-level validation for <see cref="PayDuesItemCommand"/>. Deep card
/// validity (Luhn, CVV shape, expiry) is the payment gateway's job; here we
/// only check the ids and that card fields are present.
/// </summary>
public sealed class PayDuesItemCommandValidator : AbstractValidator<PayDuesItemCommand>
{
    public PayDuesItemCommandValidator()
    {
        RuleFor(x => x.DuesPeriodId).ValidId();
        RuleFor(x => x.ItemId).ValidId();
        RuleFor(x => x.CardNumber).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Cvv).NotEmpty().WithMessage(ValidationMessages.Required);
    }
}
