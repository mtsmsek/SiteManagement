using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;

public class GetListResidentByApartmentNumberAndBlockNameQueryHandler : IRequestHandler<GetListResidentByApartmentNumberAndBlockNameQuery,
                                                                          PagedViewModel<GetListResidentByApartmentNumberAndBlockNameResponse>>
{
    private readonly IResidentRepository _residentRepository;
    private readonly IMapper _mapper;
    private readonly BlockBusinessRules _blockBusinessRules;
    private readonly ApartmentBusinessRules _apartmentBusinessRules;
    private readonly IApartmentRepository _apartmentRepository;
    public GetListResidentByApartmentNumberAndBlockNameQueryHandler(IResidentRepository residentRepository, IMapper mapper, BlockBusinessRules blockBusinessRules, IApartmentRepository apartmentRepository)
    {
        _residentRepository = residentRepository;
        _mapper = mapper;
        _blockBusinessRules = blockBusinessRules;
        _apartmentRepository = apartmentRepository;
    }

    public async Task<PagedViewModel<GetListResidentByApartmentNumberAndBlockNameResponse>> Handle(GetListResidentByApartmentNumberAndBlockNameQuery request, CancellationToken cancellationToken)
    {
        //TODO -- remove message
        //TODO -- refactor
        //TODO -- debug this function
        //TODO -- check the mapping configuration
        await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockName, "Block cannot found!");
        var apartment = await _apartmentRepository.GetSingleAsync(predicate: apartment => apartment.ApartmentNumber == request.ApartmentNumber
                                                              && apartment.Block.Name == request.BlockName,
                                                   includes: apartment => apartment.Block);

        var residents = await _residentRepository.GetListAsync(predicate: resident => resident.ApartmentId == apartment.Id,
                                                               orderBy: null,
                                                               cancellationToken: cancellationToken,
                                                               includes: [x => x.Apartment, x => x.Apartment.Block]);

        return _mapper.Map<PagedViewModel<GetListResidentByApartmentNumberAndBlockNameResponse>>(residents);


    }
    // 3 8 4 6 9 5
}
