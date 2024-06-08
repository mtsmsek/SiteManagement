using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Constants.Residents;

namespace SiteManagement.Application.Features.Queries.Residents.GetResidentsByFullName
{
    public class GetResidentsByFullNameQueryHandler : IRequestHandler<GetResidentsByFullNameQuery, PagedViewModel<GetResidentsByFullNameResponse>>
    {
        private readonly IResidentRepository _residentRepository;
        private readonly IMapper _mapper;

        public GetResidentsByFullNameQueryHandler(IResidentRepository residentRepository, IMapper mapper)
        {
            _residentRepository = residentRepository;
            _mapper = mapper;
        }

        public async Task<PagedViewModel<GetResidentsByFullNameResponse>> Handle(GetResidentsByFullNameQuery request, CancellationToken cancellationToken)
        {

            var residents = await _residentRepository.GetListAsync(predicate: resident => resident.FirstName == request.FirstName &&
                                                                      resident.LastName == request.LastName,
                                                                      cancellationToken: cancellationToken,
                                                                      includes: [resident => resident.Apartment,
                                                                                 resident => resident.Apartment.Block]);
           
            if (residents.Results.Count == 0)
                throw new BusinessException(ResidentMessages.RuleMessages.ResidentCannotBeFound);


            
            return _mapper.Map<PagedViewModel<GetResidentsByFullNameResponse>>(residents);
        }


    }
}
