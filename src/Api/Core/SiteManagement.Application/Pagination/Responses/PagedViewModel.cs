using SiteManagement.Application.Pagination.Paging;
using SiteManagement.Application.Pagination.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Pagination.Responses
{
    public class PagedViewModel<TResponse>  
    {
        public PagedViewModel() : this(new List<TResponse>(), new Page())
        {
            
        }
        public PagedViewModel(IList<TResponse> results, Page pageInfo)
        {
            Results = results;
            PageInfo = pageInfo;
        }
        public IList<TResponse> Results { get; set; }
        public Page PageInfo { get; set; }
    }
    
}
