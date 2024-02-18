using Microsoft.EntityFrameworkCore.Query;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Entities.Commons;
using System.Linq.Expressions;

namespace SiteManagement.Application.Services.Repositories.Commons;

public interface IAsyncRepository<TEntity> where TEntity : BaseEntity
                                                 
{
    Task<int> AddAsync(TEntity entity,
                       CancellationToken cancellationToken = default);
    Task<int> AddRangeAsync(IEnumerable<TEntity> entities,
                            CancellationToken cancellationToken = default);
    Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate = null, bool noTracking = true,
                                 CancellationToken cancellationToken = default,
                                 params Expression<Func<TEntity, object>>[] includes);
    Task<PagedViewModel<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null,
                                               Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                               int currentPage = 1,
                                               int pageSize = 10,
                                               bool noTracking = true,
                                               CancellationToken cancellationToken = default,
                                               params Expression<Func<TEntity, object>>[] includes);
}
