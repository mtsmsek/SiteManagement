using MediatR;
using SiteManagement.Application.Validators.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;


namespace SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;

public class UpdateBillCommand : IRequest<UpdateBillResponse>, IBillCommandToValidate
{
    public Guid Id { get; set; }
    public int Type { get; set; }
    public double Fee { get; set; }
    public bool IsPaid { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}