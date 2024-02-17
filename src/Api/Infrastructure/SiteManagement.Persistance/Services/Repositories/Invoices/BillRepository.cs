using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagement.Application.Services.Repositories.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Services.Repositories.Invoices
{
    public class BillRepository : EfAsyncRepository<Bill, SiteManagementApplicationContext>
        , IBillReposiotry
    {
        public BillRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }
    }
}
