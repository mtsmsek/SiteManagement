using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Billing.Commands.DistributeDues;

/// <summary>Surface-level validation for <see cref="DistributeDuesCommand"/>.</summary>
public sealed class DistributeDuesCommandValidator : AbstractValidator<DistributeDuesCommand>
{
    public DistributeDuesCommandValidator()
    {
        RuleFor(x => x.DuesPeriodId).ValidId();
    }
}
