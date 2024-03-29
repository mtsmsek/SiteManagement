using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlockId;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus;

namespace SiteManagement.Api.WebApi.Controllers.Buildings;

[Route("api/[controller]")]
[ApiController]
public class ApartmentsController : BaseController
{
    private const int _defaultCurrentPage = 1;
    private const int _defaultCurrentPageSize = 10;
    #region Create
    [HttpPost("createApartment")]
    public async Task<IActionResult> CreateApartment(CreateApartmentCommand createApartmentCommand)
    {
        var response = await Mediator.Send(createApartmentCommand);
        return Ok(response);
    }
    #endregion
    #region Update
    [HttpPost("changeResidentStatus")]
    public async Task<IActionResult> ChangeResidentStatus(Guid id, bool status)
    {
        var response = await Mediator.Send(new ChangeResidentStatusCommand {Id = id, Status =status  });
        return Ok(response);
    }
    [HttpPost("changeTenantStatus")]
    public async Task<IActionResult> ChangeTenantStatus(Guid id, bool isTenant)
    {
        var response = await Mediator.Send(new ChangeTenantStatusCommand { Id = id, IsTenant = isTenant});
        return Ok(response);
    }
    #endregion
    #region Delete
    [HttpDelete("hardDeleteApartment")]
    public async Task<IActionResult> HardDeleteApartment(Guid id)
    {
        var response = await Mediator!.Send(new HardDeleteApartmentCommand
        {
            Id = id
        });
        return Ok(response);
    }
    #endregion
    #region Get
    [HttpGet("getApartments{name}Block")]
    public async Task<IActionResult> GetApartmentsByBlock(Guid? id, string? name, int currentPage = _defaultCurrentPage, int pageSize = _defaultCurrentPageSize)
    {
        if(!id.HasValue)
            id = Guid.Empty;
        if(string.IsNullOrEmpty(name))
            name = string.Empty;

        var response = await Mediator.Send(new GetListAllApartmentsByBlockQuery { BlockId = id.Value, BlockName = name, Page = currentPage, PageSize = pageSize });
        return Ok(response);    
    }
    [HttpGet("getEmptyApartmentsIn{name}Block")]
    public async Task<IActionResult> GetEmptyApartmentsByBlock(string? name, int currentPage = _defaultCurrentPage, int pageSize = _defaultCurrentPageSize)
    {
        if (string.IsNullOrEmpty(name))
            name = string.Empty;

        var response = await Mediator.Send(new GetListApartmentsInBlockByStatusQuery { BlockName = name, Status = false, Page = currentPage, PageSize = pageSize });
        return Ok(response);
    }
    [HttpGet("getFullApartmentsIn{name}Block")]
    public async Task<IActionResult> GetFullApartmentsByBlock(string? name, int currentPage = _defaultCurrentPage, int pageSize = _defaultCurrentPageSize)
    {
        if (string.IsNullOrEmpty(name))
            name = string.Empty;

        var response = await Mediator.Send(new GetListApartmentsInBlockByStatusQuery { BlockName = name, Status= true,Page = currentPage, PageSize = pageSize });
        return Ok(response);
    }
    [HttpGet("emptyApartments")]
    public async Task<IActionResult> GetEmptyApartments(int currentPage = 1, int pageSize = 10)
    {
        var blocksList = await Mediator!.Send(new GetListApartmentsByStatusQuery { Status = false, Page = currentPage, PageSize = pageSize });
        return Ok(blocksList);
    }
    [HttpGet("fullApartments")]
    public async Task<IActionResult> GetFullApartments(int currentPage = 1, int pageSize = 10)
    {
        var blocksList = await Mediator!.Send(new GetListApartmentsByStatusQuery { Status = true, Page = currentPage, PageSize = pageSize });
        return Ok(blocksList);
    }
    #endregion
}