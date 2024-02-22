using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus
{
    public class GetListApartmentsByStatusQueryHandler : IRequestHandler<GetListApartmentsByStatusQuery,
                                                                          PagedViewModel<GetListApartmentsByStatusResponse>>
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;

        public GetListApartmentsByStatusQueryHandler(IApartmentRepository apartmentRepository, IMapper mapper)
        {
            _apartmentRepository = apartmentRepository;
            _mapper = mapper;
        }

        public async Task<PagedViewModel<GetListApartmentsByStatusResponse>> Handle(GetListApartmentsByStatusQuery request, CancellationToken cancellationToken)
        {
           var apartmentList = await _apartmentRepository.GetListAsync(predicate: apartment => apartment.Status == request.Status,
                                                    currentPage: request.Page,
                                                    pageSize: request.PageSize,
                                                    orderBy: x => x.OrderBy(x => x.Block.Name).ThenBy(x => x.ApartmentNumber),
                                                    includes: x => x.Block,
                                                    cancellationToken: cancellationToken) ;

           var response =  _mapper.Map<PagedViewModel<GetListApartmentsByStatusResponse>>(apartmentList);

            return response;
                    

        }
    }
}
