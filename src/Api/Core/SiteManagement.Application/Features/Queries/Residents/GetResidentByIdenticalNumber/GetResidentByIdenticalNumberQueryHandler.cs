using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Constants.Residents;

namespace SiteManagement.Application.Features.Queries.Residents.GetResidentByIdenticalNumber;

public class GetResidentByIdenticalNumberQueryHandler : IRequestHandler<GetResidentByIdenticalNumberQuery, GetResidentByIdenticalNumberResponse>
{
    private readonly IResidentRepository _residenttRepository;
    private readonly IMapper _mapper;

    public GetResidentByIdenticalNumberQueryHandler(IResidentRepository residenttRepository, IMapper mapper)
    {
        _residenttRepository = residenttRepository;
        _mapper = mapper;
    }

    public async Task<GetResidentByIdenticalNumberResponse> Handle(GetResidentByIdenticalNumberQuery request, CancellationToken cancellationToken)
    {
        var resident = await _residenttRepository.GetSingleAsync(predicate: resident => resident.IdenticalNumber == request.IdenticalNumber,
                                                                    includes: [resident => resident.Apartment,
                                                                      resident => resident.Apartment.Block]);

        if (resident is null)
            throw new BusinessException(ResidentMessages.RuleMessages.ResidentWithIdenticalNumberDoesNotExist);

        return _mapper.Map<GetResidentByIdenticalNumberResponse>(resident);
    }

}
