using FluentValidation;
using SiteManagement.Application.Validators.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;


namespace SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle;

public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(c => c.VehicleRegistrationPlate).ValidateVehicleRegistrationPlate();


        RuleFor(c => c.VehicleType).ValidateVehicleType();

    }
   

}
