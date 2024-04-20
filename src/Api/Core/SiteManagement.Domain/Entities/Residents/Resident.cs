using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Payments;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Domain.Entities.Residents;

public class Resident : User
{
    public Guid ApartmentId { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get; set; }
    
    public virtual Apartment Apartment { get; set; }

    public virtual ICollection<Message> SentMessages { get; set; }
    public virtual ICollection<Message> ReceivedMessages { get; set; }
    public virtual ICollection<ResidentVehicle> Vehicles { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }

}
