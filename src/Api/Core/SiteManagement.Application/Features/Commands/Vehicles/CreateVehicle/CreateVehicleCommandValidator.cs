using FluentValidation;
using SiteManagement.Application.Validators.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle
{
    public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleCommandValidator()
        {

            RuleFor(c => c.VehicleRegistrationPlate).ValidateVehicleRegistrationPlate();
           
            RuleFor(c => c.VehicleType).ValidateVehicleType();

        }
           
    }
}
