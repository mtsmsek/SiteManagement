using MediatR;
using SiteManagement.Application.Pagination.Requests;
using SiteManagement.Application.Pagination.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus
{
    public class GetListApartmentsInBlockByStatusQuery : PageRequest, IRequest<PagedViewModel<GetListApartmentsInBlockByStatusResponse>>
    {
        private string _name;
        public string BlockName { get => _name; set => _name = value.ToUpper(); }
        public bool Status { get; set; }
    }
}
