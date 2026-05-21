using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.OpenDuesPeriod;

/// <summary>Surface-level validation for <see cref="OpenDuesPeriodCommand"/>.</summary>
public sealed class OpenDuesPeriodCommandValidator : AbstractValidator<OpenDuesPeriodCommand>
{
    public OpenDuesPeriodCommandValidator()
    {
        RuleFor(x => x.SiteId).ValidId();
        RuleFor(x => x.PerApartmentAmount).PositiveAmount();
    }
}
