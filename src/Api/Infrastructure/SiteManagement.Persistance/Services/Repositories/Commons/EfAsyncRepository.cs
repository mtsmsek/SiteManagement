using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Entities.Commons;
using SiteManagemnt.Application.Services.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<int> AddAsync(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
