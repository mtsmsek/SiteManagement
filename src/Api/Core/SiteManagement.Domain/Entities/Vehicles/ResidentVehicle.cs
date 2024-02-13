using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Entities.Residents;

namespace SiteManagement.Domain.Entities.Vehicles;

public class ResidentVehicle : BaseEntity
{
    public Guid ResidentId { get; set; }
    public Guid VehicleId { get; set; }

    public virtual Resident Resident{ get; set; }
    public virtual Vehicle Vehicle { get; set; }

}
