using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Residents;

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
            //TODO make it and ??
            //TODO take one property named 'name' then process it here ??
            var resident = await _residentRepository.GetListAsync(predicate: resident => resident.FirstName == request.FirstName &&
                                                                      resident.LastName == request.LastName,
                                                                      cancellationToken: cancellationToken,
                                                                      includes: [resident => resident.Apartment,
                                                                                 resident => resident.Apartment.Block]);
            //todo -- remove magic string
            if (resident is null)
                throw new BusinessException("Bu isimde bir kullanıcı bulunamadı");


            //todo -- investigater how to map includes
            return _mapper.Map<PagedViewModel<GetResidentsByFullNameResponse>>(resident);
        }


    }
}
