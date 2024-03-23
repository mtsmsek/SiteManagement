using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Services.Repositories.Security
{
    public class OperationClaimRepository : EfAsyncRepository<OperationClaim, SiteManagementApplicationContext>, IOperationClaimRepository
    {
        public OperationClaimRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }
    }
}
