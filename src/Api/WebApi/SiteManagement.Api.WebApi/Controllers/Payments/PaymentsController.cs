using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Payments.CreatePayment;
using SiteManagement.Application.Features.Commands.Payments.DeletePayment.HardDeletePayment;
using SiteManagement.Application.Features.Commands.Payments.UpdatePayment;
using SiteManagement.Application.Features.Queries.Payments.GetListResidentPayments;
using SiteManagement.Application.Security.Extensions;

namespace SiteManagement.Api.WebApi.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay(CreatePaymentCommand createPaymentCommand)
        {
            var result = await Mediator!.Send(createPaymentCommand);
            return Ok(result);
        }
        [HttpPost("hardDeletePayment")]
        public async Task<IActionResult> HardDeletePayment(Guid id)
        {
            await Mediator!.Send(new HardDeletePaymentCommand { Id = id });
            return Ok();
        }

        [HttpPut("updatePaymentInfo")]
        public async Task<IActionResult> UpdatePayment(UpdatePaymentCommand updatePaymentCommand)
        {
            var result = await Mediator!.Send(updatePaymentCommand);
            return Ok(result);  
        }
        [HttpGet("payments")]
        public async Task<IActionResult> GetResidentPayments()
        {
            //todo  check and fix here
            if (_httpContextAccessor.HttpContext!.User is null)
                return BadRequest();
           var id =  _httpContextAccessor.HttpContext.User.GetUserId();

           var results = await Mediator!.Send(new GetListResidentPaymentsQuery()
            {
                UserId = id
            });
            return Ok(results);
        }

    }
}
