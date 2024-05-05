using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
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
        await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockName);

        var residents = await _residentRepository.GetListAsync(predicate: resident => resident.Apartment.Block.Name == request.BlockName,
                                                  cancellationToken: cancellationToken,
                                                  includes: [resident => resident.Apartment, resident => resident.Apartment.Block]);
        
        return _mapper.Map<PagedViewModel<GetListResidentsByBlockNameResponse>>(residents);

        
    }
}
