using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Application.Services.Repositories.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth
{
    public class GetListApartmentBillsByMonthQueryHandler : IRequestHandler<GetListApartmentBillsByMonthQuery,
                                                                            PagedViewModel<GetListApartmentBillsByMonthResponse>>
    {
        private readonly IBillReposiotry _billReposiotry;
        private readonly IMapper _mapper;
        private readonly BillBusinessRules _billBusinessRules;

        public GetListApartmentBillsByMonthQueryHandler(IBillReposiotry billReposiotry, IMapper mapper, BillBusinessRules billBusinessRules)
        {
            _billReposiotry = billReposiotry;
            _mapper = mapper;
            _billBusinessRules = billBusinessRules;
        }

        public async Task<PagedViewModel<GetListApartmentBillsByMonthResponse>> Handle(GetListApartmentBillsByMonthQuery request, 
                                                                                      CancellationToken cancellationToken)
        {
            //TODO -- Complete here after create resident functions
            throw new NotImplementedException();
        }
    }
}
