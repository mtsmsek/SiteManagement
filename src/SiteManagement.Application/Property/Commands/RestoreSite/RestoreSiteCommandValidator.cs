using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Property.Commands.RestoreSite;

/// <summary>Surface-level validation for <see cref="RestoreSiteCommand"/>.</summary>
public sealed class RestoreSiteCommandValidator : AbstractValidator<RestoreSiteCommand>
{
    public RestoreSiteCommandValidator()
    {
        RuleFor(x => x.SiteId).ValidId();
    }
}
