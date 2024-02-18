using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Application.Pagination.Paging;
using SiteManagement.Application.Pagination.Requests;
using SiteManagement.Application.Pagination.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks
{
    public class GetListAllBlockQuery : PageRequest, IRequest<PagedViewModel<GetListAllBlockResponse>>, ICachableRequest
    {
        public string CacheKey => $"GetListAllBlockQuery({Page},{PageSize})";

        public bool BypassCache { get; }

        public TimeSpan? SlidingExpiration { get; }
    }
}
