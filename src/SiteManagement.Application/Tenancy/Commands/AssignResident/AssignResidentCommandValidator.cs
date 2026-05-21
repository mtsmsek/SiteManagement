using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Tenancy.Commands.AssignResident;

/// <summary>Surface-level validation for <see cref="AssignResidentCommand"/>.</summary>
public sealed class AssignResidentCommandValidator : AbstractValidator<AssignResidentCommand>
{
    public AssignResidentCommandValidator()
    {
        RuleFor(x => x.ApartmentId).ValidId();
        RuleFor(x => x.ResidentId).ValidId();
        RuleFor(x => x.TenantType).IsInEnum();
    }
}
