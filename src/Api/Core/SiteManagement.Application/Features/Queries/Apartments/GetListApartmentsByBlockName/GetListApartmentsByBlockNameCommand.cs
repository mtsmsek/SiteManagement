using MediatR;
using SiteManagement.Application.Pagination.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByBlockName
{
    public class GetListApartmentsByBlockNameCommand : IRequest<PagedViewModel<GetListApartmentsByBlockNameResponse>>
    {
        public string BlockName { get; set; }
    }
}
