using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Entities.Payments
{
    public class Payment : BaseEntity
    {
        public Guid ResidentId { get; set; }
        public Guid BillId { get; set; }
        public virtual Resident Resident { get; set; }
        public virtual Bill Bill { get; set; }
    }
}
