using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Residency.Queries.GetResidentById;

/// <summary>Surface-level validation for <see cref="GetResidentByIdQuery"/>.</summary>
public sealed class GetResidentByIdQueryValidator : AbstractValidator<GetResidentByIdQuery>
{
    public GetResidentByIdQueryValidator()
    {
        RuleFor(x => x.ResidentId).ValidId();
    }
}
