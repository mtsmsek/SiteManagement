using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Features.Queries.Vehicles.GetVehicleByRegistrationPlate
{
    public class GetVehicleByRegistrationPlateResponse
    {
        public string VehicleRegistrationPlate { get; set; }
        public VehicleType VehicleType { get; set; }

    }
}
