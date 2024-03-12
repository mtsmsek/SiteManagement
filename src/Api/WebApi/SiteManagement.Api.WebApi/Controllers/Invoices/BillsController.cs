using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete;

namespace SiteManagement.Api.WebApi.Controllers.Invoices;

[Route("api/[controller]")]
[ApiController]
public class BillsController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateBill(CreateBillCommand createBillCommand)
    {
        var result = await Mediator!.Send(createBillCommand);
        return Ok(result);

    }
    [HttpDelete]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        var result = await Mediator!.Send(new HardDeleteBillCommand { Id = id });
        return Ok(result);
    }
}
