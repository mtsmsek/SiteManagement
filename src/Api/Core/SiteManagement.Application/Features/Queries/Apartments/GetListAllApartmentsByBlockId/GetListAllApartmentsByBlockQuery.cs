using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Application.Pagination.Requests;
using SiteManagement.Application.Pagination.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlockId
{
    public class GetListAllApartmentsByBlockQuery : PageRequest, IRequest<PagedViewModel<GetListAllApartmentsByBlockResponse>>, ICachableRequest
    {
        private string _name;
        public Guid BlockId { get; set; }
        public string? BlockName { get => _name; set => _name = value.ToUpper() ?? string.Empty; }

        public string CacheKey => $"GetAllApartments";

        public bool BypassCache { get;}

        public TimeSpan? SlidingExpiration { get; }
    }
}
