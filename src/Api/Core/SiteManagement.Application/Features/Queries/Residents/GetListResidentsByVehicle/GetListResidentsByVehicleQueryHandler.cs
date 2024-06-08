using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentsByVehicle;

public class GetListResidentsByVehicleQueryHandler : IRequestHandler<GetListResidentsByVehicleQuery,
                                                                     PagedViewModel<GetListResidentsByVehicleResponse>>
{
    private readonly IResidentRepository _residentRepository;
    private readonly IMapper _mapper;
    private readonly VehicleBusinessRules _vehicleBusinessRules;
    private readonly IResidentVehicleRepository _residentVehicleRepository;

    public GetListResidentsByVehicleQueryHandler(IResidentRepository residentRepository, IMapper mapper, VehicleBusinessRules vehicleBusinessRules, IResidentVehicleRepository residentVehicleRepository)
    {
        _residentRepository = residentRepository;
        _mapper = mapper;
        _vehicleBusinessRules = vehicleBusinessRules;
        _residentVehicleRepository = residentVehicleRepository;
    }

    public async Task<PagedViewModel<GetListResidentsByVehicleResponse>> Handle(GetListResidentsByVehicleQuery request, CancellationToken cancellationToken)
    {

        //TODO make comprasion between this algorithm and (first find vehicle and then use the resident vehicle id)


        await _vehicleBusinessRules.CheckIfVehicleExistByRegistrationPlate(request.VehicleRegistrationPlate, cancellationToken);


        var residents = await _residentRepository.GetListAsync(predicate: resident => resident.Vehicles.Any(x => x.Vehicle.VehicleRegistrationPlate == request.VehicleRegistrationPlate),
                                                                    includes: [resident => resident.Vehicles,
                                                                        resident => resident.Apartment,
                                                                        resident => resident.Apartment.Block]);

        return _mapper.Map<PagedViewModel<GetListResidentsByVehicleResponse>>(residents);

    }
}
