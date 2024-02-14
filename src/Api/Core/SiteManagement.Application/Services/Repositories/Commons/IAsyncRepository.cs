using SiteManagement.Domain.Entities.Commons;

namespace SiteManagemnt.Application.Services.Repositories.Commons;

public interface IAsyncRepository<TEntity> where TEntity : BaseEntity
                                                 
{
    Task<int> AddAsync(TEntity entity);
}
