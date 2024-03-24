using MediatR;
using SiteManagement.Domain.Enumarations.Invoices;


namespace SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;

public class UpdateBillCommand : IRequest<UpdateBillResponse>
{
    public Guid Id { get; set; }
    public Guid ApartmentId { get; set; }
    public BillType Type { get; set; }
    public double Fee { get; set; }
    public bool IsPaid { get; set; }
    public Month Month { get; set; }
    public int Year { get; set; }
}