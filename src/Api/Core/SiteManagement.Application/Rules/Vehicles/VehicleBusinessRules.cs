using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Application.Rules.Vehicles
{
    public class VehicleBusinessRules : BaseBusinessRules
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserRepository _userRepository;

        public VehicleBusinessRules(IVehicleRepository vehicleRepository, IUserRepository userRepository)
        {
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
        }

        public async Task VehiclePlateRegisterCannotDuplicateWhenInsert(string vehiclePlateRegister, CancellationToken cancellationToken)
        {
            var dbVehicle = await _vehicleRepository.GetSingleAsync(predicate: vehicle => vehicle.VehicleRegistrationPlate == vehiclePlateRegister,
                                                     cancellationToken: cancellationToken);
            //TODO -- remove magic string
            if (dbVehicle is not null)
                throw new BusinessException("Belirtilen plakalı araç sistemde mevcut");

        }
        public async Task<bool> SetVehicleStatusOfResidents(Guid userGuid, CancellationToken cancellationToken)
        {
            var currentTime = DateTime.Now;

            var user = await _userRepository.GetByIdAsync(userGuid, cancellationToken: cancellationToken);
            TimeSpan difference = currentTime - user.BirthDate;
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
                throw new BusinessException("Vehicle cannot found");

            return dbVehicle;
        }
        public async Task<Vehicle> CheckIfVehicleExistByRegistrationPlate(string  registrationPlate, CancellationToken cancellationToken)
        {
            var dbVehicle = await _vehicleRepository.GetSingleAsync(predicate: vehicle => vehicle.VehicleRegistrationPlate == registrationPlate);

            if (dbVehicle is null)
                throw new BusinessException("Bu plakaya ait bir araç bulunamadı");

            return dbVehicle;
        }
    }
}
