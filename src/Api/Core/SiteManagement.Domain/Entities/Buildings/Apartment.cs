using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Enumarations.Buildings;

namespace SiteManagement.Domain.Entities.Buildings;

public class Apartment : BaseEntity
{
    public Guid BlockId { get; set; }
    public bool Status { get; set; }
    public ApartmentType ApartmentType { get; set; }
    public int ApartmentNumber { get; set; }
    public int FloorNumber { get; set; }
    public bool IsTenant { get; set; }

    public virtual Block Block { get; set; }
}
