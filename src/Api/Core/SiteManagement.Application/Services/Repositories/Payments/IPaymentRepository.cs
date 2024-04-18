using SiteManagement.Application.Services.Repositories.Commons;
using SiteManagement.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Services.Repositories.Payments
{
    public interface IPaymentRepository : IAsyncRepository<Payment>
    {
    }
}
