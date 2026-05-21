using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.CloseUtilityBillPeriod;

/// <summary>Surface-level validation for <see cref="CloseUtilityBillPeriodCommand"/>.</summary>
public sealed class CloseUtilityBillPeriodCommandValidator : AbstractValidator<CloseUtilityBillPeriodCommand>
{
    public CloseUtilityBillPeriodCommandValidator()
    {
        RuleFor(x => x.UtilityBillPeriodId).ValidId();
    }
}
