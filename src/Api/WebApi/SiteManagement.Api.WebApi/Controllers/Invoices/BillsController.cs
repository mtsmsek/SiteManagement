using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;
using SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete;
using SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;
using SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth;

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
    #region Get
    [HttpGet("getApartmentBills")]
    public async Task<IActionResult> GetApartmentBillsByMonth(Guid apartmentId, int? month, int? year, int? billType)
    {
        var result = await Mediator!.Send(new GetListApartmentBillsByMonthQuery
        {
            ApartmentId = apartmentId,
            Month = month.HasValue ? month : null,
            Year = year.HasValue ? year : null,
            BillType = billType.HasValue ? billType : null
        }); 
        return Ok(result);
    }
    #endregion
}
