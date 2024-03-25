using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Features.Queries.Vehicles.GetListVehicles;

public class GetListAllVehiclesResponse
{
    public string VehicleRegistrationPlate { get; set; }
    public VehicleType VehicleType { get; set; }
}
