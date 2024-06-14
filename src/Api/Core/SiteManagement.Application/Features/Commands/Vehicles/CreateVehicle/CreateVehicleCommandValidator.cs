using FluentValidation;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle
{
    public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
    {
        //register plate => 34 ABC 285
        private string _provincePart = string.Empty; //34
        private string _middlePart = string.Empty; //ABC
        private string _lastPart = string.Empty; //285
        public CreateVehicleCommandValidator()
        {
            RuleFor(c => c.VehicleRegistrationPlate).NotEmpty().WithMessage(VehicleMessages.ValidationMessages.RegistraionPlateCannotBeEmpty)
               .Must(PlateMustBeConsistFrom3Part).WithMessage(VehicleMessages.ValidationMessages.InvalidRegistrationPlate)
               .Must(c => ProvincePartMustBeBetween1And81(_provincePart)).WithMessage(VehicleMessages.ValidationMessages.InvalidProvincePart);


            RuleFor(c => c.VehicleType).NotEmpty().WithMessage(VehicleMessages.ValidationMessages.VehicleTypeCannotBeEmpty)
                .Must(VehicleTypeShouldBeExist).WithMessage(VehicleMessages.ValidationMessages.InvalidVehicleType);

        }
        private bool VehicleTypeShouldBeExist(int vehicleType)
        {
            return VehicleType.Enumarations.ContainsKey(vehicleType);
        }
        private bool PlateMustBeConsistFrom3Part(string vehicleRegistrationPlate)
        {
            var splittedPlate = vehicleRegistrationPlate.Split(' ');

            if (splittedPlate.Length == 3)
            {
                _provincePart = splittedPlate[0];
                _middlePart = splittedPlate[1];
                _lastPart = splittedPlate[2];
                return true;
            }

            return false;

        }

        private bool ProvincePartMustBeBetween1And81(string provincePart)
        {
            if(int.TryParse(provincePart, out int provinceNumber))
            {
                return provinceNumber >= 1 && provinceNumber <= 81;
            }
            return false;
        }



           
    }
}
