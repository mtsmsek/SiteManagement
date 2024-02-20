using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagement.Application.Services.Repositories.Buildings;
using Bogus;

namespace SiteManagement.Persistance.Services.Repositories.Buildings
{
    public class BlockRepository : EfAsyncRepository<Block, SiteManagementApplicationContext>,
        IBlockRepository
    {
        public BlockRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<Block> IsBlockExist(Guid id)
                => await GetByIdAsync(id);

        public async Task<Block> IsBlockExist(string name)
                => await GetSingleAsync(block => block.Name == name);

        public async Task<bool> IsBlockNameUnique(string name)
             => await GetSingleAsync(block => block.Name == name) is null;
        
    }
}
