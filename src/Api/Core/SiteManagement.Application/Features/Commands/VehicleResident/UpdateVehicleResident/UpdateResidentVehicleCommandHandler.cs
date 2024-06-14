using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Rules.ResidentVehicles;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Constants.Vehicles;

namespace SiteManagement.Application.Features.Commands.VehicleResident.UpdateVehicleResident;

public class UpdateResidentVehicleCommandHandler : IRequestHandler<UpdateResidentVehicleCommand, bool>
{
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly ResidentBusinessRules _residentBusinessRules;
    private readonly ResidentVehicleBusinessRules _residentVehicleBusinessRules;
    private readonly VehicleBusinessRules _vehicleBusinessRules;

    public UpdateResidentVehicleCommandHandler(IResidentVehicleRepository residentVehicleRepository, ResidentBusinessRules residentBusinessRules, ResidentVehicleBusinessRules residentVehicleBusinessRules, VehicleBusinessRules vehicleBusinessRules)
    {
        _residentVehicleRepository = residentVehicleRepository;
        _residentBusinessRules = residentBusinessRules;
        _residentVehicleBusinessRules = residentVehicleBusinessRules;
        _vehicleBusinessRules = vehicleBusinessRules;
    }

    public async Task<bool> Handle(UpdateResidentVehicleCommand request, CancellationToken cancellationToken)
    {
        var residentVehicle = await _residentVehicleBusinessRules.CheckIfResidentVehicleExistById(request.Id, cancellationToken);

        var resident = await _residentBusinessRules.CheckIfResidentExistById(id: request.UserId, cancellationToken: cancellationToken);


        await _vehicleBusinessRules.CheckIfVehiceExistById(id: request.VehicleId, cancellationToken: cancellationToken);


        residentVehicle.DriveStatus = _vehicleBusinessRules.SetVehicleStatusOfResidents(resident.BirthDate,cancellationToken);

        await _residentVehicleRepository.UpdateAsync(residentVehicle);

        return residentVehicle.DriveStatus; 
        
    }
}
