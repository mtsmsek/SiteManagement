using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Extensions;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;
using System.Linq.Expressions;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;

public class GetListResidentsByApartmentNumberAndBlockNameQueryHandler : IRequestHandler<GetListResidentsByApartmentNumberAndBlockNameQuery,
                                                                          PagedViewModel<GetListResidentsByApartmentNumberAndBlockNameResponse>>
{
    private readonly IResidentRepository _residentRepository;
    private readonly IMapper _mapper;
    private readonly BlockBusinessRules _blockBusinessRules;
    private readonly IApartmentRepository _apartmentRepository;
    public GetListResidentsByApartmentNumberAndBlockNameQueryHandler(IResidentRepository residentRepository, IMapper mapper, BlockBusinessRules blockBusinessRules, IApartmentRepository apartmentRepository)
    {
        _residentRepository = residentRepository;
        _mapper = mapper;
        _blockBusinessRules = blockBusinessRules;
        _apartmentRepository = apartmentRepository;
    }

    public async Task<PagedViewModel<GetListResidentsByApartmentNumberAndBlockNameResponse>> Handle(GetListResidentsByApartmentNumberAndBlockNameQuery request, CancellationToken cancellationToken)
    {


        await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockName);

        var residents = await _residentRepository.GetListAsync(predicate: resident => resident.Apartment.Block.Name == request.BlockName &&
                                                               resident.Apartment.ApartmentNumber == request.ApartmentNumber,
                                                               cancellationToken: cancellationToken,
                                                               includes: [x => x.Apartment, x => x.Apartment.Block]);

        return _mapper.Map<PagedViewModel<GetListResidentsByApartmentNumberAndBlockNameResponse>>(residents);


    }

}
