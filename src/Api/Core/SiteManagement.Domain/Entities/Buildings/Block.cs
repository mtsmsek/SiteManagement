using SiteManagement.Domain.Entities.Commons;

namespace SiteManagement.Domain.Entities.Buildings;

public class Block : BaseEntity
{
    public string Name { get; set; }
    public virtual ICollection<Apartment> Apartments { get; set; }

}
