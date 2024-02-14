using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagemnt.Application.Services.Repositories.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Services.Repositories.Residents
{
    public class ResidentRepository : EfAsyncRepository<Resident, SiteManagementApplicationContext>,
        IResidentRepository
    {
        public ResidentRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }
    }
}
