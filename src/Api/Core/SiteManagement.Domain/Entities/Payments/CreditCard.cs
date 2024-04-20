using SiteManagement.Domain.Entities.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Entities.Payments
{
    public class CreditCard : BaseEntity
    {
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpireDate { get; set; }
        public string CVCNumber { get; set; }
        public double Amount { get; set; }

    }
}
