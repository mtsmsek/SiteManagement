using FluentValidation;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Validators.Vehicles;

public static class BaseVehicleValidatorExtensions
{
    //register plate => 34 ABC 285
    private static string _provincePart = string.Empty; //34
    private static string _middlePart = string.Empty; //ABC
    private static string _lastPart = string.Empty; //285
    public static void ValidateVehicleRegistrationPlate<TVehicleCommand>(this IRuleBuilderInitial<TVehicleCommand, string> ruleBuilder)
        where TVehicleCommand : IVehicleCommand
    {
        ruleBuilder.NotEmpty().WithMessage(VehicleMessages.ValidationMessages.RegistraionPlateCannotBeEmpty)
               .Must(PlateMustBeConsistFrom3Part).WithMessage(VehicleMessages.ValidationMessages.InvalidRegistrationPlate)
               .Must(c => ProvincePartMustBeBetween1And81(_provincePart)).WithMessage(VehicleMessages.ValidationMessages.InvalidProvincePart);


    }

    public static void ValidateVehicleType<TVehicleCommand>(this IRuleBuilderInitial<TVehicleCommand, int> ruleBuilder)
       where TVehicleCommand : IVehicleCommand
    {

        ruleBuilder.NotEmpty().WithMessage(VehicleMessages.ValidationMessages.VehicleTypeCannotBeEmpty)
              .Must(VehicleTypeShouldBeExist).WithMessage(VehicleMessages.ValidationMessages.InvalidVehicleType);
    }

    private static bool VehicleTypeShouldBeExist(int vehicleType)
    {
        return VehicleType.Enumarations.ContainsKey(vehicleType);
    }
    private static bool PlateMustBeConsistFrom3Part(string vehicleRegistrationPlate)
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

    private static bool ProvincePartMustBeBetween1And81(string provincePart)
    {
        if (int.TryParse(provincePart, out int provinceNumber))
        {
            return provinceNumber >= 1 && provinceNumber <= 81;
        }
        return false;
    }
}
