using SiteManagement.Domain.Primitives;

namespace SiteManagement.Domain.Enumarations.Vehicles;

public class VehicleType : Enumaration<VehicleType>
{
    public static readonly VehicleType Car = new(1, "Car");
    public static readonly VehicleType Motorcycle = new(2, "Motorcycle");
    public VehicleType(int value, string name) : base(value, name)
    {
    }
}
