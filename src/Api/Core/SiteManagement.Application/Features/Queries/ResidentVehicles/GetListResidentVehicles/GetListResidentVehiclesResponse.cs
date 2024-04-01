using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Features.Queries.ResidentVehicles.GetListResidentVehicles
{
    public class GetListResidentVehiclesResponse
    {
        //todo more properties ??
       
        public string VehicleRegistrationPlate { get; set; }
        public VehicleType VehicleType { get; set; }
    }
    
}
