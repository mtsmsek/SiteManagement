using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.OpenUtilityBillPeriod;

/// <summary>Surface-level validation for <see cref="OpenUtilityBillPeriodCommand"/>.</summary>
public sealed class OpenUtilityBillPeriodCommandValidator : AbstractValidator<OpenUtilityBillPeriodCommand>
{
    public OpenUtilityBillPeriodCommandValidator()
    {
        RuleFor(x => x.SiteId).ValidId();
        RuleFor(x => x.TotalAmount).PositiveAmount();
        RuleFor(x => x.UtilityType).IsInEnum();
    }
}
