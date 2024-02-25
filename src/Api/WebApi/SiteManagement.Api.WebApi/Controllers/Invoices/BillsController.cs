using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;

namespace SiteManagement.Api.WebApi.Controllers.Invoices
{
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
    }
}
