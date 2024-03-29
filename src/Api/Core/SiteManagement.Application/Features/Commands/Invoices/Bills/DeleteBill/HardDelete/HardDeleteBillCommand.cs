using MediatR;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete;

public class HardDeleteBillCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}
