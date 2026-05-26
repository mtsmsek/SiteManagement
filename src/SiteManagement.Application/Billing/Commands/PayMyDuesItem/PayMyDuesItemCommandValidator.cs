using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.PayMyDuesItem;

/// <summary>
/// Surface-level validation for <see cref="PayMyDuesItemCommand"/>. Shares the
/// card rules with every other pay path; ownership is the handler's job.
/// </summary>
public sealed class PayMyDuesItemCommandValidator : AbstractValidator<PayMyDuesItemCommand>
{
    public PayMyDuesItemCommandValidator()
    {
        RuleFor(x => x.DuesPeriodId).ValidId();
        RuleFor(x => x.ItemId).ValidId();
        RuleFor(x => x.CardNumber).ValidCardNumber();
        RuleFor(x => x.Cvv).ValidCvv();
    }
}
