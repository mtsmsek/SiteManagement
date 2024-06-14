using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;

namespace SiteManagement.Application.Features.Queries.ResidentVehicles.GetListResidentVehicles;

public class GetListResidentVehiclesQueryHandler : IRequestHandler<GetListResidentVehiclesQuery, PagedViewModel<GetListResidentVehiclesResponse>>
{
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly IMapper _mapper;

    public GetListResidentVehiclesQueryHandler(IResidentVehicleRepository residentVehicleRepository, IMapper mapper)
    {
        _residentVehicleRepository = residentVehicleRepository;
        _mapper = mapper;
    }

    public async Task<PagedViewModel<GetListResidentVehiclesResponse>> Handle(GetListResidentVehiclesQuery request, CancellationToken cancellationToken)
    {
        var residentVehicles = await _residentVehicleRepository.GetListAsync(predicate: residentVehicle => residentVehicle.ResidentId == request.ResidentId,
                                       includes: [residentVehicle => residentVehicle.Vehicle,
                                     residentVehicle => residentVehicle.Resident]);
       
        if (residentVehicles.Results.Count == 0)
            throw new BusinessException(ResidentVehicleMessages.RuleMessages.ResidentOrVehicleCannotBeFound);

        return _mapper.Map<PagedViewModel<GetListResidentVehiclesResponse>>(residentVehicles);
    }
}
