using MediatR;
using SiteManagement.Application.Pagination.Requests;
using SiteManagement.Application.Pagination.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Residents.GetListAllResidents
{
    public class GetListAllResidentsQuery : PageRequest,IRequest<PagedViewModel<GetListAllResidentsResponse>>
    {
    }
}
