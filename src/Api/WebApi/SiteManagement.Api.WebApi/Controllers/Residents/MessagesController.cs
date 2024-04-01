using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Messages.DeleteMessage.HardDelete;
using SiteManagement.Application.Features.Commands.Messages.SendMessage;
using SiteManagement.Application.Features.Queries.Messaages.GetResidentMessages;

namespace SiteManagement.Api.WebApi.Controllers.Residents;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : BaseController
{
    //todo -- test it after migration
    #region Create
    [HttpPost("sendMessage")]
    public async Task<IActionResult> SendMessage(SendMessageCommand sendMessageCommand)
    {
        var result = await Mediator!.Send(sendMessageCommand);
        return Ok(result);
    }
    #endregion
    #region Delete
    [HttpDelete("hardDeleteMessage")]
    public async Task<IActionResult> HardDeleteMessage(Guid id)
    {
        //todo -- from body ??
        var result = await Mediator!.Send(new HardDeleteMessageCommand
        {
            MessageId = id
        });
        return Ok(result);
    }
    #endregion
    #region Get
    [HttpGet("getResidentMessages")]
    public async Task<IActionResult> GetResidentMessages(Guid userId)
    {
        var result = await Mediator!.Send(new GetResidentMessagesQuery
        {
            UserId = userId
        });

        return Ok(result);
    }
    #endregion 

}
