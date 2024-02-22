﻿using AutoMapper;
using MediatR;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus
{
    public class GetListApartmentsInBlockByStatusQueryHandler : IRequestHandler<GetListApartmentsInBlockByStatusQuery,
                                                                                PagedViewModel<GetListApartmentsInBlockByStatusResponse>>
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;

        public GetListApartmentsInBlockByStatusQueryHandler(IApartmentRepository apartmentRepository, IMapper mapper)
        {
            _apartmentRepository = apartmentRepository;
            _mapper = mapper;
        }

        public async Task<PagedViewModel<GetListApartmentsInBlockByStatusResponse>> Handle(GetListApartmentsInBlockByStatusQuery request, CancellationToken cancellationToken)
        {
            var apartmentList = await _apartmentRepository.GetListAsync(predicate: apartment =>
                                                                        apartment.Block.Name == request.BlockName &&
                                                                        apartment.Status == request.Status,
                                                                        currentPage: request.Page,
                                                                        pageSize: request.PageSize,
                                                                        orderBy: x => x.OrderBy(x => x.ApartmentNumber),
                                                                        includes: x => x.Block,
                                                                        cancellationToken: cancellationToken);

            var response = _mapper.Map<PagedViewModel<GetListApartmentsInBlockByStatusResponse>>(apartmentList);

            return response;
        }
    }
}
