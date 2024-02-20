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
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, bool noTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _entity;

            if (noTracking)
                query = query.AsNoTracking();

            if(predicate is not null)
                query = query.Where(predicate);

            return await query.AnyAsync(cancellationToken);

        }
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
        public async Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity found = await _entity.FindAsync(id, cancellationToken);
            
            if (found == null)
                return null;

            if (noTracking)
                _dbContext.Entry(found).State = EntityState.Detached;

            foreach (Expression < Func<TEntity, object>> include in includes)
            {
                _dbContext.Entry(found).Reference(include).Load();
            }

            return found;
        }
        #endregion
        #region Update
        public async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
           _entity.Entry(entity).State = EntityState.Modified;
            entity.UpdatedDate = DateTime.UtcNow;
            _entity.Update(entity);

            return await _dbContext.SaveChangesAsync();
        }
        #endregion
        #region Delete
        public async Task<int> DeleteAsync(TEntity entity, bool isPermenant = false, CancellationToken cancellationToken = default)
        {
            await SetEntityAsDeletedAsync(entity, isPermenant);
            return await _dbContext.SaveChangesAsync();
        }

        #endregion
        #region Helper Methods
        protected async Task SetEntityAsDeletedAsync(TEntity entity, bool isPermenant)
        {
            if(!isPermenant)
            {
                //TODO -- Soft delete will create here!
            }
            else
            {
                _dbContext.Remove(entity);
            }
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

      
        #endregion

    }
}
