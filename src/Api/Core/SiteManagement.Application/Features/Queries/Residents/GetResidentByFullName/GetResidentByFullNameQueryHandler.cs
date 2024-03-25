using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Queries.Residents.GetResidentByFullName
{
    public class GetResidentByFullNameQueryHandler : IRequestHandler<GetResidentByFullNameQuery, GetResidentByFullNameResponse>
    {
        private readonly IResidentRepository _residentRepository;
        private readonly IMapper _mapper;
        public async Task<GetResidentByFullNameResponse> Handle(GetResidentByFullNameQuery request, CancellationToken cancellationToken)
        {
            //TODO make it and ??
            //TODO take one property named 'name' then process it here ??
            var resident = await _residentRepository.GetSingleAsync(predicate: resident => resident.FirstName == request.FirstName ||
                                                                      resident.LastName == request.LastName,
                                                                      includes: [resident => resident.Apartment,
                                                                                 resident => resident.Apartment.Block]);
            //todo -- remove magic string
            if (resident is null)
                throw new BusinessException("Bu isimde bir kullanıcı bulunamadı");


            //todo -- investigater how to map includes
            return _mapper.Map<GetResidentByFullNameResponse>(resident);
        }


    }
}
