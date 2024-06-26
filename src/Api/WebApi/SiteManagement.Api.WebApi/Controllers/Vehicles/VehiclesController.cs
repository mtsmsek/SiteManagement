﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;
using SiteManagement.Application.Features.Commands.Vehicles.DeleteCehicle.HardDelete;
using SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle;
using SiteManagement.Application.Features.Queries.Vehicles.GetListVehicles;
using SiteManagement.Application.Features.Queries.Vehicles.GetVehicleByRegistrationPlate;

namespace SiteManagement.Api.WebApi.Controllers.Vehicles
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : BaseController
    {
        #region Create
        [HttpPost("createVehicle")]
        public async Task<IActionResult> CreateVehicle(CreateVehicleCommand createVehicleCommand)
        {
            var result = await Mediator!.Send(createVehicleCommand);
            return Ok(result);
        }
        #endregion
        #region Delete
        [HttpDelete("hardDeleteVehicle")]
        public async Task<IActionResult> HardDeleteVehicle(HardDeleteVehicleCommand hardDeleteVehicleCommand)
        {
            //todo -- both guid id can be taken from query string
            var result = await Mediator!.Send(hardDeleteVehicleCommand);
            return Ok(result);
        }
        #endregion
        #region Update
        [HttpPost("updateVehicle")]
        public async Task<IActionResult> UpdateVehicle(UpdateVehicleCommand updateVehicleCommand)
        {
            var result = await Mediator!.Send(updateVehicleCommand);
            return Ok(result);  
        }
        #endregion
        #region Get
        [HttpGet("allVehicles")]
        public async Task<IActionResult> GetAllVehicles()
        {
            var result = await Mediator!.Send(new GetListAllVehiclesQuery());
            return Ok(result);
        }
        [HttpGet("getVehicleByRegistrationPlate")]
        public async Task<IActionResult> GetAllVehicles(string registrationPlate)
        {
            var result = await Mediator!.Send(new GetVehicleByRegistrationPlateQuery
            {
                VehicleRegistrationPlate = registrationPlate
            });
            return Ok(result);
        }
        #endregion

    }
}
