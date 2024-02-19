using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;
using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;

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
        [HttpGet]
        public async Task<IActionResult> GetAllBlocks(int currentPage, int PageSize)
        {
            var blocksList = await Mediator!.Send(new GetListAllBlockQuery { Page = currentPage, PageSize = PageSize});
            return Ok(blocksList);  
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBlockName(UpdateBlockNameCommand updateBlockNameCommand)
        {
            var updatedBlock = await Mediator.Send(updateBlockNameCommand);
            return Ok(updatedBlock);
        }
    }
}
