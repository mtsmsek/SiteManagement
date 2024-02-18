using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Application.Services.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SiteManagement.Domain.Utulity;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Pagination.Paging;

namespace SiteManagement.Persistance.Services.Repositories.Commons
{
    public class EfAsyncRepository<TEntity, TContext> : IAsyncRepository<TEntity>
        where TEntity : BaseEntity
        where TContext : DbContext
    {
        private readonly DbContext _dbContext;
        protected DbSet<TEntity> _entity => _dbContext.Set<TEntity>();
        public EfAsyncRepository(TContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddAsync(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities is null && !entities.Any())
                return 0;
            
            await _dbContext.AddRangeAsync(entities);
            return await _dbContext.SaveChangesAsync();
        }
        #region Get Methods
        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate = null, bool noTracking = true,
                                                  CancellationToken cancellationToken = default,
                                                  params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entity;
            if(predicate is not null)
                query = query.Where(predicate);

            
             query = ApplyIncludes(query, includes);

            if(noTracking)
                query = query.AsNoTracking();

            
            return await query.SingleOrDefaultAsync();



        }
        #endregion



        public Task<PagedViewModel<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null,                                                                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                          int currentPage = 1, 
                                                          int pageSize = 10, 
                                                          bool noTracking = true,
                                                          CancellationToken cancellationToken = default,
                                                          params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entity;
            if(predicate is not null)
                query = query.Where(predicate);

            if (orderBy is not null)
                orderBy(query);

            if(noTracking)
              query =  query.AsNoTracking();
          
            query = ApplyIncludes(query, includes);

            return query.PaginateAsync(currentPage,pageSize,cancellationToken);
        }
        private IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes is not null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            return query;
        }
    }
}
