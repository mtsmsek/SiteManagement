using SiteManagement.Domain.Entities.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Entities.Buildings;

public class Apartment : BaseEntity
{
    public Guid BlockId { get; set; }
    public bool Status { get; set; }
    //TODO create enumaration for apartment type
    public int ApartmentNumber { get; set; }
    public int FloorNumber { get; set; }
    public bool IsTenant { get; set; }
}
