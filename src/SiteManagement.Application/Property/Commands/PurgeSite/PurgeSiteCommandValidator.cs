using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Property.Commands.PurgeSite;

/// <summary>Surface-level validation for <see cref="PurgeSiteCommand"/>.</summary>
public sealed class PurgeSiteCommandValidator : AbstractValidator<PurgeSiteCommand>
{
    public PurgeSiteCommandValidator()
    {
        RuleFor(x => x.SiteId).ValidId();
    }
}
