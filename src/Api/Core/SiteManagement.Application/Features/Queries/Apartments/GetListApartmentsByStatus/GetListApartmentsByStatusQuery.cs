using MediatR;
using SiteManagement.Application.Pagination.Requests;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Enumarations.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus
{
    public class GetListApartmentsByStatusQuery : PageRequest, IRequest<PagedViewModel<GetListApartmentsByStatusResponse>>
    {
        public bool Status { get; set; }
    }
}
