using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Application.Services.Repositories.Commons;

namespace SiteManagement.Application.Services.Repositories.Buildings;

public interface IBlockRepository : IAsyncRepository<Block>
{
    Task<bool> IsBlockNameUnique(string name);
    Task<Block> IsBlockExist(Guid id);
    Task<Block> IsBlockExist(string name);
}
