using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Residency.Commands.AddVehicle;

/// <summary>Surface-level validation for <see cref="AddVehicleCommand"/>.</summary>
public sealed class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.ResidentId).ValidId();
        RuleFor(x => x.Plate).ValidPlateNumber();
        // Note is optional; the domain rejects oversize notes with a typed exception.
    }
}
