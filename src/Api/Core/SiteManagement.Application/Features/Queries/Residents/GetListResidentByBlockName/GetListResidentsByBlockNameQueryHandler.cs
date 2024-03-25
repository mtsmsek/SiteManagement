using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByBlockName;

public class GetListResidentsByBlockNameQueryHandler : IRequestHandler<GetListResidentsByBlockNameQuery,
                                                                   PagedViewModel<GetListResidentsByBlockNameResponse>>
{
    private readonly IResidentRepository _residentRepository;
    private readonly IMapper _mapper;
    private readonly BlockBusinessRules _blockBusinessRules;

    public GetListResidentsByBlockNameQueryHandler(IResidentRepository residentRepository, IMapper mapper, BlockBusinessRules blockBusinessRules)
    {
        _residentRepository = residentRepository;
        _mapper = mapper;
        _blockBusinessRules = blockBusinessRules;
    }

    public async Task<PagedViewModel<GetListResidentsByBlockNameResponse>> Handle(GetListResidentsByBlockNameQuery request, CancellationToken cancellationToken)
    {
        //TODO -- remove the message from business rules
        //TODO -- check if includes and algorthm true
        var blockToSearch = await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockName, "Block cannot be found");

        var residents = _residentRepository.GetListAsync(predicate: resident => blockToSearch.Apartments.Contains(resident.Apartment),
                                                         cancellationToken: cancellationToken,
                                                         includes: [resident => resident.Apartment, resident => resident.Apartment.Block]);

        return _mapper.Map<PagedViewModel<GetListResidentsByBlockNameResponse>>(residents);

        
    }
}
