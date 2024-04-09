using FluentValidation;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle;

public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    //todo-- create base vehicle validator
    //register plate => 34 BZD 285
    private string _provincePart = string.Empty; //34
    private string _middlePart = string.Empty; //BZD
    private string _lastPart = string.Empty; //285
    public UpdateVehicleCommandValidator()
    {
        RuleFor(c => c.VehicleRegistrationPlate).NotEmpty().WithMessage("Plaka boş geçilemez")
           .Must(PlateMustBeConsistFrom3Part).WithMessage("Geçersiz plaka bilgisi")
           .Must(ProvincePartMustBeBetween1And81).WithMessage("Geçersiz il bilgisi");


        RuleFor(c => c.VehicleType).NotEmpty().WithMessage("Lütfen araç tipini seçiniz")
            .Must(VehicleTypeShouldBeExist).WithMessage("Geçersiz araç türü");

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
        if (int.TryParse(provincePart, out int provinceNumber))
        {
            return provinceNumber >= 1 && provinceNumber <= 81;
        }
        return false;
    }

}
