using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagemnt.Application.Services.Repositories.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
