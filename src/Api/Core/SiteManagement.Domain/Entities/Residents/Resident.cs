using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Domain.Entities.Residents;

public class Resident : User
{
    public Guid ApartmentId { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get; set; }
    
    public virtual Apartment Apartment { get; set; }

    public virtual ICollection<Message > Messages { get; set; }
    public virtual ICollection<ResidentVehicle> Vehicles { get; set; }

}
