using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Pagination.Requests
{
    public class PageRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
