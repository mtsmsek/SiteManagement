using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Payments.GetListResidentPayments
{
    public class GetListResidentPaymentsResponse
    {

        public string BillType { get; set; }
        public double Fee { get; set; }
        public string PersonWhoPaid { get; set; }
    }
}
