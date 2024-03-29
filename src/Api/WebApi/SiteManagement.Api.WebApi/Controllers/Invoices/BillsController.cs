using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;
using SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete;
using SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;

namespace SiteManagement.Api.WebApi.Controllers.Invoices;

[Route("api/[controller]")]
[ApiController]
public class BillsController : BaseController
{
    #region Create
    [HttpPost("createBill")]
    public async Task<IActionResult> CreateBill(CreateBillCommand createBillCommand)
    {
        var result = await Mediator!.Send(createBillCommand);
        return Ok(result);

    }
    [HttpPost("createBulkBills")]
    public async Task<IActionResult> CreateBulkBills(CreateBulkBillsCommand createBulkBillsCommand)
    {
        var result = await Mediator!.Send(createBulkBillsCommand);
        return Ok(result);
    }
    #endregion
    #region Delete
    [HttpDelete("hardDeleteBill")]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        var result = await Mediator!.Send(new HardDeleteBillCommand { Id = id });
        return Ok(result);
    }
    #endregion
    #region Update
    [HttpPost("updateBill")]
    public async Task<IActionResult> UpdateBill(UpdateBillCommand updateBillCommand)
    {
        var result = await Mediator!.Send(updateBillCommand);
        return Ok(result);
    }
    #endregion
}
