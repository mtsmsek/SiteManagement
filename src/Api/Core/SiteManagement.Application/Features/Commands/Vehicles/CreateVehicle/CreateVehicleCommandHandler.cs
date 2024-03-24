using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;
//TODO -- control all mappings
//TODO -- control all securitty functions
public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, CreateVehicleResponse>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly IResidentRepository _residentRepository;
    private readonly VehicleBusinessRules _vehicleBusinessRules;
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    public CreateVehicleCommandHandler(IVehicleRepository vehicleRepository, IMapper mapper, IResidentRepository residentRepository, VehicleBusinessRules vehicleBusinessRules, IResidentVehicleRepository residentVehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _residentRepository = residentRepository;
        _vehicleBusinessRules = vehicleBusinessRules;
        _residentVehicleRepository = residentVehicleRepository;
    }

    public async Task<CreateVehicleResponse> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        //TODO - add validation
        var vehicleToAdd = _mapper.Map<Vehicle>(request);
        await _vehicleRepository.AddAsync(vehicleToAdd);

        var residents = await _residentRepository.GetListAsync(predicate: resident => resident.ApartmentId == request.ApartmentId);
        List<ResidentVehicle> residentVehicles = new();
        foreach(var resident in residents.Results) 
        {

             var canResidentDrive = await _vehicleBusinessRules.SetVehicleStatusOfResidents(resident.Id, cancellationToken);
            var vehicleResidentToAdd = new ResidentVehicle()
            {
                ResidentId = resident.Id,
                VehicleId = vehicleToAdd.Id,
                DriveStatus = canResidentDrive
            };
           residentVehicles.Add(vehicleResidentToAdd);
        }


        //TODO -- remove magic strinng
        if (residentVehicles.Count == 0)
            throw new BusinessException("Plaka eklemek istediğiniz tüm kullanıcılar 18 yaşından küçük");

        //TODO-- make this ITransactional request
        await _residentVehicleRepository.AddRangeAsync(residentVehicles, cancellationToken);

        return _mapper.Map<CreateVehicleResponse>(vehicleToAdd);

    }
}
