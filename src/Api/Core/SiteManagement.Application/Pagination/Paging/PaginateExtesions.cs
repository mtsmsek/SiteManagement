using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Pagination.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Pagination.Paging
{
    public static class PaginateExtesions
    {
        public static async Task<PagedViewModel<TResponse>> PaginateAsync<TResponse>(this IQueryable<TResponse> query,
                                                                                int currentPage, 
                                                                                int pageSize, 
                                                                                CancellationToken cancellationToken = default)
        {
            int totalRowCount = await query.CountAsync();
            Page page = new(currentPage, pageSize, totalRowCount);

            var data = await query.Skip(page.Skip).Take(pageSize).ToListAsync();

            var result = new PagedViewModel<TResponse>(data, page);
            return result;
        }
    }
}
