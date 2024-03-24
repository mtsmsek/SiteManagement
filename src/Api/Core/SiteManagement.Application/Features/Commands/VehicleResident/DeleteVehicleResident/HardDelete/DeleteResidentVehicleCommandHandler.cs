using MediatR;
using SiteManagement.Application.Rules.ResidentVehicles;
using SiteManagement.Application.Services.Repositories.Vehicles;

namespace SiteManagement.Application.Features.Commands.VehicleResident.DeleteVehicleResident.HardDelete;

public class DeleteResidentVehicleCommandHandler : IRequestHandler<DeleteResidentVehicleCommand, int>
{
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly ResidentVehicleBusinessRules _residentVehicleBusinessRules;

    public DeleteResidentVehicleCommandHandler(IResidentVehicleRepository residentVehicleRepository, ResidentVehicleBusinessRules residentVehicleBusinessRules)
    {
        _residentVehicleRepository = residentVehicleRepository;
        _residentVehicleBusinessRules = residentVehicleBusinessRules;
    }

    public async Task<int> Handle(DeleteResidentVehicleCommand request, CancellationToken cancellationToken)
    {
        //TODO -- Create all validations
       var residentVehicleToAdd =  await _residentVehicleBusinessRules.CheckIfResidentVehicleExistById(request.ResidentId, cancellationToken);

        return await _residentVehicleRepository.DeleteAsync(residentVehicleToAdd,
                                                            isPermenant: true,
                                                            cancellationToken);
    }
}
