using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.MarkDuesItemPaid;

/// <summary>Surface-level validation for <see cref="MarkDuesItemPaidCommand"/>.</summary>
public sealed class MarkDuesItemPaidCommandValidator : AbstractValidator<MarkDuesItemPaidCommand>
{
    public MarkDuesItemPaidCommandValidator()
    {
        RuleFor(x => x.DuesPeriodId).ValidId();
        RuleFor(x => x.ItemId).ValidId();
    }
}
