using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Domain.Entities.Residents;

public class Resident : BaseEntity
{
    public Guid ApartmentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public virtual Apartment Apartment { get; set; }
    public virtual ICollection<ResidentVehicle> Vehicles { get; set; }

}
