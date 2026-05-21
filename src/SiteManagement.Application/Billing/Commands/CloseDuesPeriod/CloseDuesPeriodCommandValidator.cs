using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.CloseDuesPeriod;

/// <summary>Surface-level validation for <see cref="CloseDuesPeriodCommand"/>.</summary>
public sealed class CloseDuesPeriodCommandValidator : AbstractValidator<CloseDuesPeriodCommand>
{
    public CloseDuesPeriodCommandValidator()
    {
        RuleFor(x => x.DuesPeriodId).ValidId();
    }
}
