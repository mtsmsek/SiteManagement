using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagement.Application.Services.Repositories.Buildings;

namespace SiteManagement.Persistance.Services.Repositories.Buildings
{
    public class BlockRepository : EfAsyncRepository<Block, SiteManagementApplicationContext>,
        IBlockRepository
    {
        public BlockRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }
    }
}
