using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete;
using SiteManagement.Application.Features.Commands.Security.OperationClaims.CreateOperationClaim;
using SiteManagement.Application.Features.Commands.Security.OperationClaims.UpdateOperationClaim;
using SiteManagement.Application.Features.Queries.Security.OperationClaims.GetListAllOperationClaims;
using SiteManagement.Application.Features.Queries.Security.OperationClaims.GetOperationClaimByName;

namespace SiteManagement.Api.WebApi.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationClaimsController : BaseController
    {
        [HttpPost("createOperationClaim")]
        public async Task<IActionResult> CreateOperationClaim(CreateOperationClaimCommand createOperationClaimCommand)
        {
            var result = await Mediator!.Send(createOperationClaimCommand);
            return Ok(result);
        }

        [HttpDelete("hardDeleteOperationClaim")] 
        public async Task<IActionResult> HardDeleteOperationClaim(HardDeleteApartmentCommand hardDeleteApartmentCommand)
        {
            var result = await Mediator!.Send(hardDeleteApartmentCommand);
            return Ok(result);  
        }
        [HttpPut("updateOperationClaim")]
        public async Task<IActionResult> UpdateOperationClaim(UpdateOperationClaimCommand updateOperationClaimCommand)
        {
            var result = await Mediator!.Send(updateOperationClaimCommand);
            return Ok(result);
        }

        [HttpGet("opetaionClaims")]
        public async Task<IActionResult> GetAllOperationClaims()
        {
            var result = await Mediator!.Send(new GetListAllOperationClaimsQuery());
            return Ok(result);
        }
        //todo research here
        [HttpGet("operationClaim{name}")]
        public async Task<IActionResult> GetOperationClaimByName(string name)
        {
            var result = await Mediator!.Send(new GetOperationClaimByNameQuery
            {
                Name = name
            });
            return Ok(result);
        }
    }
}
