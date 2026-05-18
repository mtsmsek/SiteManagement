using FluentValidation;
using SiteManagement.Application.Shared.Validation;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.AddApartment;

/// <summary>
/// Surface-level validation. Range checks on Number / Floor live on the
/// value objects too; doing them here as well surfaces a 400 before the
/// handler runs.
/// </summary>
public sealed class AddApartmentCommandValidator : AbstractValidator<AddApartmentCommand>
{
    public AddApartmentCommandValidator()
    {
        RuleFor(x => x.BlockId).ValidId();
        RuleFor(x => x.Number).InRange(PropertyLimits.ApartmentNumberMin, PropertyLimits.ApartmentNumberMax);
        RuleFor(x => x.Floor).InRange(PropertyLimits.FloorMin, PropertyLimits.FloorMax);
        RuleFor(x => x.Type).RequiredText();
    }
}
