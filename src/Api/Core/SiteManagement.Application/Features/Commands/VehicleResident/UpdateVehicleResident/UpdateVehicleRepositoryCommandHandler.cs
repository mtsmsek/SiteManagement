using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Rules.ResidentVehicles;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Vehicles;

namespace SiteManagement.Application.Features.Commands.VehicleResident.UpdateVehicleResident;

public class UpdateResidentVehicleCommandHandler : IRequestHandler<UpdateResidentVehicleCommand, bool>
{
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly ResidentBusinessRules _residentBusinessRules;
    private readonly VehicleBusinessRules _vehicleBusinessRules;
    private readonly ResidentVehicleBusinessRules _residentVehicleBusinessRules;

    public UpdateResidentVehicleCommandHandler(IResidentVehicleRepository residentVehicleRepository, ResidentBusinessRules residentBusinessRules, VehicleBusinessRules vehicleBusinessRules, ResidentVehicleBusinessRules residentVehicleBusinessRules)
    {
        _residentVehicleRepository = residentVehicleRepository;
        _residentBusinessRules = residentBusinessRules;
        _vehicleBusinessRules = vehicleBusinessRules;
        _residentVehicleBusinessRules = residentVehicleBusinessRules;
    }

    public async Task<bool> Handle(UpdateResidentVehicleCommand request, CancellationToken cancellationToken)
    {
        var residentVehicle = await _residentVehicleBusinessRules.CheckIfResidentVehicleExistById(request.Id, cancellationToken);

       var resident =  await _residentBusinessRules.CheckIfResidentExistById(request.VehicleId, cancellationToken);
       await _vehicleBusinessRules.CheckIfVehiceExistById(request.VehicleId, cancellationToken);

        

       residentVehicle.DriveStatus =  await _vehicleBusinessRules.SetVehicleStatusOfResidents(resident.Id,cancellationToken);

        await _residentVehicleRepository.UpdateAsync(residentVehicle);

        return residentVehicle.DriveStatus; 
        
    }
}
