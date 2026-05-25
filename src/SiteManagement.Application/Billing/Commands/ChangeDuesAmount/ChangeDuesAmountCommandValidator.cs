using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.ChangeDuesAmount;

/// <summary>Surface-level validation for <see cref="ChangeDuesAmountCommand"/>.</summary>
public sealed class ChangeDuesAmountCommandValidator : AbstractValidator<ChangeDuesAmountCommand>
{
    public ChangeDuesAmountCommandValidator()
    {
        RuleFor(x => x.DuesPeriodId).ValidId();
        RuleFor(x => x.PerApartmentAmount).PositiveAmount();
    }
}
