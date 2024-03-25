using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Vehicles;

namespace SiteManagement.Application.Features.Queries.Vehicles.GetListVehicles;

public class GetListAllVehiclesQueryHandler : IRequestHandler<GetListAllVehiclesQuery,
                                                              PagedViewModel<GetListAllVehiclesResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public GetListAllVehiclesQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<PagedViewModel<GetListAllVehiclesResponse>> Handle(GetListAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        //todo check if there will be an include or not
        var vehicles = await _vehicleRepository.GetListAsync();

        return _mapper.Map<PagedViewModel<GetListAllVehiclesResponse>>(vehicles);
    }
}
