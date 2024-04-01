using SiteManagement.Domain.Enumarations.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth
{
    public class GetListApartmentBillsByMonthResponse
    {
        public string BlockName { get; set; }
        public string ApartmentNumber { get; set; }
        public string BillType { get; set; }
        public double Fee { get; set; }
        public bool IsPaid { get; set; }
        public string Period { get; set; }
    }
}
