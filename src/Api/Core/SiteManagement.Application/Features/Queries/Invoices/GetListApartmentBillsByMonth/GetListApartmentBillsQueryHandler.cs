using AutoMapper;
using MediatR;
using SiteManagement.Application.Extensions;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;
using System.Linq.Expressions;

namespace SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth
{
    public class GetListApartmentBillsQueryHandler : IRequestHandler<GetListApartmentBillsQuery,
                                                                            PagedViewModel<GetListApartmentBillsResponse>>
    {
        private readonly IBillReposiotry _billReposiotry;
        private readonly IMapper _mapper;
        private readonly BillBusinessRules _billBusinessRules;
        private readonly ApartmentBusinessRules _apartmentBusinessRules;

        public GetListApartmentBillsQueryHandler(IBillReposiotry billReposiotry, IMapper mapper, BillBusinessRules billBusinessRules, ApartmentBusinessRules apartmentBusinessRules)
        {
            _billReposiotry = billReposiotry;
            _mapper = mapper;
            _billBusinessRules = billBusinessRules;
            _apartmentBusinessRules = apartmentBusinessRules;
        }

        public async Task<PagedViewModel<GetListApartmentBillsResponse>> Handle(GetListApartmentBillsQuery request,
                                                                                      CancellationToken cancellationToken)
        {
            await _apartmentBusinessRules.ApartmentShouldExistInDatabase(request.ApartmentId, cancellationToken);
            Expression<Func<Bill,bool>> predicate = bill => bill.ApartmentId == request.ApartmentId;
            //todo -- make it generic function to do it foreach loop
            if (request.Month.HasValue)
                predicate = predicate.And(bill => bill.Month == Month.FromValue(request.Month.Value!));

            if(request.Year.HasValue)
                predicate = predicate.And(bill => bill.Year == request.Year.Value!);

            if(request.BillType.HasValue)
                predicate = predicate.And(bill => bill.Type == BillType.FromValue(request.BillType.Value!));

            var bills = await _billReposiotry.GetListAsync(predicate: predicate,
                                                 includes: [bill => bill.Apartment, bill => bill.Apartment.Block]);

            return _mapper.Map<PagedViewModel<GetListApartmentBillsResponse>>(bills);


        }
    }
}
