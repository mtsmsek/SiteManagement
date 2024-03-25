using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Residents.GetListAllResidents
{
    public class GetListAllResidentResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BlockName { get; set; }
        public int FloorNumber { get; set; }
        public int ApartmentNumber { get; set; }
        public string IdenticalNumber { get; set; }
        public string PhoneNumber { get; set; }
    }
}
