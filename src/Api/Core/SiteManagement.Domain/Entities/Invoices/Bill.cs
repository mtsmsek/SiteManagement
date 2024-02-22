﻿using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Enumarations.Invoices;

namespace SiteManagement.Domain.Entities.Invoices;

public class Bill : BaseEntity
{
    public Guid ApartmentId { get; set; }
    public BillType Type { get; set; }
    public double Fee { get; set; }
    public bool IsPaid { get; set;}
    public virtual Apartment Apartment { get; set; }

}
