using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Enumarations.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;

public class CreateBulkBillsResponse
{
    public Month Month{ get; set; }
    public int Year { get; set; }
    public double Fee { get; set; }
}
