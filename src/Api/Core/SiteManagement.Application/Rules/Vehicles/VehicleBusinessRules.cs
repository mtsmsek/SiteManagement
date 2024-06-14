using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Application.Rules.Vehicles
{
    public class VehicleBusinessRules : BaseBusinessRules
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IResidentRepository _residentRepository;

        public VehicleBusinessRules(IVehicleRepository vehicleRepository, IResidentRepository residentRepository)
        {
            _vehicleRepository = vehicleRepository;
            _residentRepository = residentRepository;
        }

        public async Task VehiclePlateRegisterCannotDuplicateWhenInsert(string vehiclePlateRegister, CancellationToken cancellationToken)
        {
            var dbVehicle = await _vehicleRepository.GetSingleAsync(predicate: vehicle => vehicle.VehicleRegistrationPlate == vehiclePlateRegister,
                                                     cancellationToken: cancellationToken);
            //TODO -- remove magic string
            if (dbVehicle is not null)
                throw new BusinessException(VehicleMessages.RuleMessages.RegistrationPlateAlreadyExist);

        }
        public bool SetVehicleStatusOfResidents(DateTime userBirthDate, CancellationToken cancellationToken)
        {
            var currentTime = DateTime.Now;

           
            TimeSpan difference = currentTime - userBirthDate;
            int age = (int)(difference.TotalDays / 365);

            if (age >= 18)
                return true;

            else
                return false;

            
        }

        public async Task<Vehicle> CheckIfVehiceExistById(Guid id, CancellationToken cancellationToken)
        {
            var dbVehicle = await _vehicleRepository.GetByIdAsync(id, cancellationToken: cancellationToken);

            if (dbVehicle is null)
                throw new BusinessException(VehicleMessages.RuleMessages.VehicleCannotFound);

            return dbVehicle;
        }
        public async Task<Vehicle> CheckIfVehicleExistByRegistrationPlate(string  registrationPlate, CancellationToken cancellationToken)
        {
            var dbVehicle = await _vehicleRepository.GetSingleAsync(predicate: vehicle => vehicle.VehicleRegistrationPlate == registrationPlate);

            if (dbVehicle is null)
                throw new BusinessException(VehicleMessages.RuleMessages.NoVehicleFoundBelongToIndicatedRegistrationPlate);

            return dbVehicle;
        }
    }
}
