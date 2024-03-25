using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;

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
        var vehicle = await _vehicleBusinessRules.CheckIfVehicleExistByRegistrationPlate(request.VehicleRegistrationPlate, cancellationToken);
        
        var residentVehicles = await _residentVehicleRepository.GetListAsync(predicate: residentVehicle => residentVehicle.VehicleId == vehicle.Id,
                                                                        includes: residentVehicle => residentVehicle.Resident);


        //TODo -- check if working and how to do this with automapper
        var results = residentVehicles.Results.Select(residentVehicle => new GetListResidentsByVehicleResponse
        {
            FirstName = residentVehicle.Resident.FirstName,
            LastName = residentVehicle.Resident.LastName,
            ApartmentNumber = residentVehicle.Resident.Apartment.ApartmentNumber,
            BlockName = residentVehicle.Resident.Apartment.Block.Name,
            FloorNumber = residentVehicle.Resident.Apartment.FloorNumber,
            IdenticalNumber = residentVehicle.Resident.IdenticalNumber,
            PhoneNumber = residentVehicle.Resident.PhoneNumber
        });

        //todo -- refactor here
        return new PagedViewModel<GetListResidentsByVehicleResponse>
        {
            Results = results.ToArray()
        };
    }
}
