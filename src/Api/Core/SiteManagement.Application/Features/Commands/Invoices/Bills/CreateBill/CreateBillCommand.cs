using MediatR;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;

public class CreateBillCommand : IRequest<Guid>
{
    public Guid ApartmentId { get; set; }
    public int Type { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }   
    public double Fee { get; set; }
}
