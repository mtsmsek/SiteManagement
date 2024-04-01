namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName
{
    public class GetListResidentsByApartmentNumberAndBlockNameResponse
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
