using SiteManagement.Domain.Enumarations.Buildings;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlock
{
    public class GetListAllApartmentsByBlockResponse 
    {
        public string BlockName { get; set; }
        public bool Status { get; set; }
        public ApartmentType ApartmentType { get; set; }
        public int ApartmentNumber { get; set; }
        public int FloorNumber { get; set; }
        public bool IsTenant { get; set; }
    }
}
