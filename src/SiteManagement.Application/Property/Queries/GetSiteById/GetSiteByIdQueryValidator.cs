using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Property.Queries.GetSiteById;

/// <summary>Surface-level validation for <see cref="GetSiteByIdQuery"/>.</summary>
public sealed class GetSiteByIdQueryValidator : AbstractValidator<GetSiteByIdQuery>
{
    public GetSiteByIdQueryValidator()
    {
        RuleFor(x => x.SiteId).ValidId();
    }
}
