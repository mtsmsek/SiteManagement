using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;

namespace SiteManagement.Api.WebApi.Controllers.Buildings
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocksController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> AddBlock(CreateBlockCommand createBlockCommand)
        {
            //TODO -- Create BadRequest
            var apartmentToAdd = await Mediator!.Send(createBlockCommand);
            
            return Ok(apartmentToAdd);
        }
    }
}
