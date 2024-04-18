using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Security.UserOperationClaims.CreateUserOperationClaim;
using SiteManagement.Application.Features.Commands.Security.UserOperationClaims.DeleteUserOperationClaim.HardDeleteUserOperationClaim;
using SiteManagement.Application.Features.Commands.Security.UserOperationClaims.UpdateUserOperationClaim;

namespace SiteManagement.Api.WebApi.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOperationClaimsController : BaseController
    {
        [HttpPost("createUserOperationClaim")]
        public async Task<IActionResult> CreateUserOperationClaim(CreateUserOperationClaimCommand createUserOperationClaimCommand)
        {
            var result = await Mediator!.Send(createUserOperationClaimCommand);
            return Ok(result);
        }

        [HttpDelete("hardDeleteUserOperationClaim")]
        public async Task<IActionResult> HardDeleteUserOperationClaim(Guid id)
        {
            await Mediator!.Send(new HardDeleteUserOperationClaimCommand{ Id = id});
            return Ok();
        }
        [HttpPost("updateUserOperationClaim")]
        public async Task<IActionResult> UpdateUserOperationClaim(UpdateUserOperationClaimCommand updateUserOperationClaimCommand)
        {
            var result = await Mediator!.Send(updateUserOperationClaimCommand);
            return Ok(result);
        }
    }
}
