using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Domain.Entities.Vehicles;

public class Vehicle : BaseEntity
{
    public string VehicleRegistrationPlate { get; set; }
    public VehicleType VehicleType { get; set; }
    public virtual ICollection<ResidentVehicle> Residents { get; set; }
}
