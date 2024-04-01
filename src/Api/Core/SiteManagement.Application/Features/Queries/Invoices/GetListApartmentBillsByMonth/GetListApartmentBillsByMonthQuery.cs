using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Enumarations.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth
{
    public class GetListApartmentBillsByMonthQuery : IRequest<PagedViewModel<GetListApartmentBillsByMonthResponse>>
    {
        public Guid ApartmentId { get; set; }
        public int? BillType { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
