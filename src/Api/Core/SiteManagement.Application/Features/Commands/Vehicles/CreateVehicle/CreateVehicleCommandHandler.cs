using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Pipelines.Transaction;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, CreateVehicleResponse>, ITransactionalRequest
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly IResidentRepository _residentRepository;
    private readonly VehicleBusinessRules _vehicleBusinessRules;
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly ApartmentBusinessRules _apartmentBusinessRules;
    public CreateVehicleCommandHandler(IVehicleRepository vehicleRepository, IMapper mapper, IResidentRepository residentRepository, VehicleBusinessRules vehicleBusinessRules, IResidentVehicleRepository residentVehicleRepository, ApartmentBusinessRules apartmentBusinessRules)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _residentRepository = residentRepository;
        _vehicleBusinessRules = vehicleBusinessRules;
        _residentVehicleRepository = residentVehicleRepository;
        _apartmentBusinessRules = apartmentBusinessRules;
    }

    public async Task<CreateVehicleResponse> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        await _apartmentBusinessRules.ApartmentShouldExistInDatabase(request.ApartmentId, cancellationToken);
        var vehicleToAdd = _mapper.Map<Vehicle>(request);
        await _vehicleRepository.AddAsync(vehicleToAdd);

        var residents = await _residentRepository.GetListAsync(predicate: resident => resident.ApartmentId == request.ApartmentId);
            
            if(residents.Results.Count == 0)
                throw new BusinessException(VehicleMessages.RuleMessages.ThereIsNoResidentLivingInApartment);


        List<ResidentVehicle> residentVehicles = new();
        foreach(var resident in residents.Results) 
        {

            var canResidentDrive = _vehicleBusinessRules.SetVehicleStatusOfResidents(resident.BirthDate, cancellationToken);
            var vehicleResidentToAdd = new ResidentVehicle()
            {
                ResidentId = resident.Id,
                VehicleId = vehicleToAdd.Id,
                DriveStatus = canResidentDrive
            };
           residentVehicles.Add(vehicleResidentToAdd);
        }

        await _residentVehicleRepository.AddRangeAsync(residentVehicles, cancellationToken);

        return _mapper.Map<CreateVehicleResponse>(vehicleToAdd);

    }
}
