using SiteManagement.Domain.Primitives;

namespace SiteManagement.Domain.Enumarations.Invoices;

public class BillType : Enumaration<BillType>
{
    public static readonly BillType Electricity = new(1, "Electricity");
    public static readonly BillType WaterBill = new(2, "Water");
    public static readonly BillType NaturalGas = new(3, "NaturalGas");
    public static readonly BillType MaintenanceFee = new(4, "MaintenanceFee");
    
    private BillType(int value, string name) : base(value, name)
    {
       
    }
}
