using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.DistributeUtilityBill;

/// <summary>Surface-level validation for <see cref="DistributeUtilityBillCommand"/>.</summary>
public sealed class DistributeUtilityBillCommandValidator : AbstractValidator<DistributeUtilityBillCommand>
{
    public DistributeUtilityBillCommandValidator()
    {
        RuleFor(x => x.UtilityBillPeriodId).ValidId();
    }
}
