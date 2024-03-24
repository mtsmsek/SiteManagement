using SiteManagement.Domain.Enumarations.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;

public class UpdateBillResponse
{
    public Guid ApartmentId { get; set; }
    public BillType Type { get; set; }
    public double Fee { get; set; }
    public bool IsPaid { get; set; }
    public string Period { get; set; } = string.Empty;
}
