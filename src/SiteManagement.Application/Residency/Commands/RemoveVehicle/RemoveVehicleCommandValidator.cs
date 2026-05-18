using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Residency.Commands.RemoveVehicle;

/// <summary>Surface-level validation for <see cref="RemoveVehicleCommand"/>.</summary>
public sealed class RemoveVehicleCommandValidator : AbstractValidator<RemoveVehicleCommand>
{
    public RemoveVehicleCommandValidator()
    {
        RuleFor(x => x.ResidentId).ValidId();
        RuleFor(x => x.Plate).ValidPlateNumber();
    }
}
