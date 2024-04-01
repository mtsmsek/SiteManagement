using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Vehicles;

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
        var residentVehicles = await _residentVehicleRepository.GetListAsync(predicate: residentVehicle => residentVehicle.ResidentId ==                                                                                                                            request.ResidentId,
                                                                                 includes: [residentVehicle => residentVehicle.Vehicle,
                                                                                            residentVehicle => residentVehicle.Resident]);
        //todo remove it ??
        if (residentVehicles is null)
            throw new BusinessException("Aradığınız kullanıcıya ait araç bulunamadı");

        return _mapper.Map<PagedViewModel<GetListResidentVehiclesResponse>>(residentVehicles);
    }
}
