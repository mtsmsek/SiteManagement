using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Application.Features.Commands.VehicleResident.CreateVehicleResident;

public class CreateVehicleResidentCommandHandler : IRequestHandler<CreateResidentVehicleCommand, int>
{
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly ResidentBusinessRules _residentBusinessRules;
    private readonly VehicleBusinessRules _vehicleResidentRules;
    private readonly IMapper _mapper;

    public CreateVehicleResidentCommandHandler(IResidentVehicleRepository residentVehicleRepository, ResidentBusinessRules residentBusinessRules, VehicleBusinessRules vehicleResidentRules, IMapper mapper)
    {
        _residentVehicleRepository = residentVehicleRepository;
        _residentBusinessRules = residentBusinessRules;
        _vehicleResidentRules = vehicleResidentRules;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateResidentVehicleCommand request, CancellationToken cancellationToken)
    {

        //TODO -- Create all controllers
        //TODO -- Create mappings
        await _residentBusinessRules.CheckIfResidentExistById(request.ResidentId, cancellationToken);
        await _vehicleResidentRules.CheckIfVehiceExistById(request.VehicleId, cancellationToken);
        var residentVehicleToAdd = _mapper.Map<ResidentVehicle>(request);

        residentVehicleToAdd.DriveStatus = await _vehicleResidentRules.SetVehicleStatusOfResidents(request.ResidentId, cancellationToken);

        return await _residentVehicleRepository.AddAsync(residentVehicleToAdd);
    }
}
