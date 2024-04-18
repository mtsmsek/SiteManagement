using SiteManagement.Domain.Entities.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Entities.Payments
{
    public class Payment : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ApartmentId { get; set; }
        public Guid BillId { get; set; }

    }
}
