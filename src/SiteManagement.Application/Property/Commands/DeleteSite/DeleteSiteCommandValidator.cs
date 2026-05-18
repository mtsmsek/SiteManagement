using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Property.Commands.DeleteSite;

/// <summary>Surface-level validation for <see cref="DeleteSiteCommand"/>.</summary>
public sealed class DeleteSiteCommandValidator : AbstractValidator<DeleteSiteCommand>
{
    public DeleteSiteCommandValidator()
    {
        RuleFor(x => x.SiteId).ValidId();
    }
}
