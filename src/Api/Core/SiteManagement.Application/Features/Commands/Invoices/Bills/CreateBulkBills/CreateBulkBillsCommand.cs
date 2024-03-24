using MediatR;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;

public class CreateBulkBillsCommand : IRequest<PagedViewModel<CreateBulkBillsResponse>>
{
    public IEnumerable<CreateBillCommand> Bills { get; set; }
}
