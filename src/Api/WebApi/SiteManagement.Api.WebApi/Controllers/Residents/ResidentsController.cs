using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Features.Commands.Residents.Login;

namespace SiteManagement.Api.WebApi.Controllers.Residents
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentsController : BaseController
    {
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
    }
}
