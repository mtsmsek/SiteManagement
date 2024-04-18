using SiteManagement.Domain.Entities.Payments;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Services.Repositories.Payments
{
    public class PaymentRepository : EfAsyncRepository<Payment, SiteManagementPaymentsContext>
    {
        public PaymentRepository(SiteManagementPaymentsContext dbContext) : base(dbContext)
        {
        }
    }
}
