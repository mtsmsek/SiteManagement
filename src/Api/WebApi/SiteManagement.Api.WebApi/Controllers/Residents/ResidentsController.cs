using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Features.Commands.Residents.DeleteResident.HardDelete;
using SiteManagement.Application.Features.Commands.Residents.Login;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword;

namespace SiteManagement.Api.WebApi.Controllers.Residents;

[Route("api/[controller]")]
[ApiController]
//TODO -- add bad resposne and others for if necessary for all controllers
public class ResidentsController : BaseController
{
    #region Create
    [HttpPost("login")]
    public async Task<IActionResult> Login(ResidentLoginCommand residentLoginCommand)
    {
        var response = await Mediator!.Send(residentLoginCommand);
         
        return Ok(response);
    }
    [HttpPost("residentCreate")]
    public async Task<IActionResult> CreateResident(CreateResidentCommand createResidentCommand)
    {
        var response = await Mediator!.Send(createResidentCommand);
        return Ok(response);
    }
    #endregion
    #region Delete
    public async Task<IActionResult> HardDeleteResident(Guid id)
    {
        var response = await Mediator!.Send(new HardDeleteResidentCommand { Id = id });
        return Ok(response);
    }
    #endregion
    #region Update
    [HttpPost("updatePassword")]
    public async Task<IActionResult> UpdatePassword(UpdateResidentPasswordCommand updateResidentPasswordCommand)
    {
        //todo remove magic string
         await Mediator!.Send(updateResidentPasswordCommand);
         return Ok("Başarı ile gerçekleşti");
    }
    [HttpPost("updateInformation")]
    public async Task<IActionResult> UpdateInformation(UpdateResidentCommand updateResidentCommand)
    {
       var result =  await Mediator!.Send(updateResidentCommand);
        return Ok(result);
    }
    [HttpPost("updateEmail")]
    public async Task<IActionResult> UpdateEmail(UpdateResidentEmailCommand updateResidentEmailCommand)
    {
        var result = await Mediator!.Send(updateResidentEmailCommand);
        return Ok(result);
    }
    #endregion

}
