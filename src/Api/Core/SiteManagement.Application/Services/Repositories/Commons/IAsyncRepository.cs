using SiteManagement.Domain.Entities.Commons;

namespace SiteManagement.Application.Services.Repositories.Commons;

public interface IAsyncRepository<TEntity> where TEntity : BaseEntity
                                                 
{
    Task<int> AddAsync(TEntity entity);
    Task<int> AddRangeAsync(IEnumerable<TEntity> entities);
}
