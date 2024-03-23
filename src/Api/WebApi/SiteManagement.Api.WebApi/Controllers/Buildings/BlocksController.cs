using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.DeleteBlock.HardDelete;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;
using SiteManagement.Application.Security.Extensions;

namespace SiteManagement.Api.WebApi.Controllers.Buildings;

[Route("api/[controller]")]
[ApiController]
public class BlocksController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> AddBlock(CreateBlockCommand createBlockCommand)
    {
        //TODO -- Create BadRequest
        var a = HttpContext.User.ClaimRoles();
        var apartmentToAdd = await Mediator!.Send(createBlockCommand);
        return Ok(apartmentToAdd);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBlockName(UpdateBlockNameCommand updateBlockNameCommand)
    {
        var updatedBlock = await Mediator.Send(updateBlockNameCommand);
        return Ok(updatedBlock);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteBlockPermenantly(Guid id)
    {
        var blockToDelete = await Mediator.Send(new HardDeleteBlockCommand { Id = id });
        return Ok(blockToDelete);
    }
    [HttpGet("blocks")]
    public async Task<IActionResult> GetAllBlocks(int currentPage, int PageSize)
    {
        var blocksList = await Mediator!.Send(new GetListAllBlockQuery { Page = currentPage, PageSize = PageSize });
        return Ok(blocksList);
    }
    [HttpGet("blockDetail")]
    public async Task<IActionResult> GetBlockDetailByName(string name)
    {
        var blocksList = await Mediator!.Send(new GetBlockDetailByNameQuery { Name = name });
        return Ok(blocksList);
    }

}


