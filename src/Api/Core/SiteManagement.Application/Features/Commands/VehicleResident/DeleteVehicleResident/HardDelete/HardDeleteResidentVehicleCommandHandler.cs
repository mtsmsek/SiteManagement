using MediatR;
using SiteManagement.Application.Rules.ResidentVehicles;
using SiteManagement.Application.Services.Repositories.Vehicles;

namespace SiteManagement.Application.Features.Commands.VehicleResident.DeleteVehicleResident.HardDelete;

public class HardDeleteResidentVehicleCommandHandler : IRequestHandler<HardDeleteResidentVehicleCommand, int>
{
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly ResidentVehicleBusinessRules _residentVehicleBusinessRules;

    public HardDeleteResidentVehicleCommandHandler(IResidentVehicleRepository residentVehicleRepository, ResidentVehicleBusinessRules residentVehicleBusinessRules)
    {
        _residentVehicleRepository = residentVehicleRepository;
        _residentVehicleBusinessRules = residentVehicleBusinessRules;
    }

    public async Task<int> Handle(HardDeleteResidentVehicleCommand request, CancellationToken cancellationToken)
    {
       
       var residentVehicleToAdd =  await _residentVehicleBusinessRules.CheckIfResidentVehicleExistById(request.ResidentVehicleId, cancellationToken);

        return await _residentVehicleRepository.DeleteAsync(residentVehicleToAdd,
                                                            isPermenant: true,
                                                            cancellationToken);
    }
}
