using MediatR;
using SiteManagement.Domain.Enumarations.Invoices;


namespace SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;

public class UpdateBillCommand : IRequest<UpdateBillResponse>
{
    public Guid Id { get; set; }
    public int Type { get; set; }
    public double Fee { get; set; }
    public bool IsPaid { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}