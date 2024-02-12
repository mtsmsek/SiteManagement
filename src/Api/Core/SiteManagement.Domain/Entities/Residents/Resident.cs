using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Commons;

namespace SiteManagement.Domain.Entities.Residents;

public class Resident : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string VehicleRegistrationPlate { get; set; }

    public virtual Apartment Apartment { get; set; }

}
