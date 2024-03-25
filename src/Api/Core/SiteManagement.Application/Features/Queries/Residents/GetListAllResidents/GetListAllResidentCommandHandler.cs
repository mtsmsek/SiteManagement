using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Residents.GetListAllResidents
{
    public class GetListAllResidentCommandHandler : IRequestHandler<GetListAllResidentCommand,
                                                                     PagedViewModel<GetListAllResidentResponse>>
    {
        private readonly IResidentRepository _residentRepository;
        private readonly IMapper _mapper;

        public GetListAllResidentCommandHandler(IResidentRepository residentRepository, IMapper mapper)
        {
            _residentRepository = residentRepository;
            _mapper = mapper;
        }

        public async Task<PagedViewModel<GetListAllResidentResponse>> Handle(GetListAllResidentCommand request, CancellationToken cancellationToken)
        {
            var residents = await _residentRepository.GetListAsync(includes: include => include.Apartment);

            var residentsResponse = _mapper.Map<PagedViewModel<GetListAllResidentResponse>>(residents);

            return residentsResponse;
        }
    }
}
