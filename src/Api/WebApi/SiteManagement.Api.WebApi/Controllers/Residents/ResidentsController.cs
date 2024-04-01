using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Features.Commands.Residents.DeleteResident.HardDelete;
using SiteManagement.Application.Features.Commands.Residents.Login;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword;
using SiteManagement.Application.Features.Queries.Residents.GetListAllResidents;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentByBlockName;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentsByVehicle;

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
    [HttpDelete("hardDeleteResident")]
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
    [HttpPut("updateInformation")]
    public async Task<IActionResult> UpdateInformation(UpdateResidentCommand updateResidentCommand)
    {
       var result =  await Mediator!.Send(updateResidentCommand);
        return Ok(result);
    }
    [HttpPut("updateEmail")]
    public async Task<IActionResult> UpdateEmail(UpdateResidentEmailCommand updateResidentEmailCommand)
    {
        var result = await Mediator!.Send(updateResidentEmailCommand);
        return Ok(result);
    }
    #endregion
    #region Get
    [HttpGet("residents")]
    public async Task<IActionResult> GetListAllResidents(int currentPage = 1, int pageSize = 10)
    {
        var result = await Mediator!.Send(new GetListAllResidentsQuery
        {
            Page = currentPage,
            PageSize = pageSize
        });
        return Ok(result);
    }
    [HttpGet("residentsBy")]
    public async Task<IActionResult> GetResidentsByApartmentNumberAndBlockName(string blockName, int apartmentNumber) 
    {
        var result = await Mediator!.Send(new GetListResidentsByApartmentNumberAndBlockNameQuery
        {
            ApartmentNumber = apartmentNumber,
            BlockName = blockName
        });

        return Ok(result);
    }
    [HttpGet("residentsBy={blockName}")]
    public async Task<IActionResult> GetResidentsByBlockName(string blockName)
    {
        var result = await Mediator!.Send(new GetListResidentsByBlockNameQuery
        {
            BlockName = blockName
        });

        return Ok(result);
    }
    [HttpGet("residents-{vehicleRegistrationPlate}")]
    public async Task<IActionResult> GetListResidentsByVehicleRegistrationPlate(string vehicleRegistrationPlate)
    {
        var result = await Mediator!.Send(new GetListResidentsByVehicleQuery
        {
            VehicleRegistrationPlate = vehicleRegistrationPlate

        });
        return Ok(result);
    }
    #endregion


}
