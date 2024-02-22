using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus
{
    public class GetListApartmentsInBlockByStatusResponse
    {
        public string BlockName { get; set; }
        public bool Status { get; set; }
        public string ApartmentType { get; set; }
        public int ApartmentNumber { get; set; }
        public int FloorNumber { get; set; }
        public bool IsTenant { get; set; }
    }
}
