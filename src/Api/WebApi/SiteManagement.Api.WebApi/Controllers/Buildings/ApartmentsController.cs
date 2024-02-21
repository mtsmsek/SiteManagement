using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.WebApi.Controllers.Commons;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlockId;

namespace SiteManagement.Api.WebApi.Controllers.Buildings
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentsController : BaseController
    {
        private const int _defaultCurrentPage = 1;
        private const int _defaultCurrentPageSize = 10;

        [HttpPost("createApartment")]
        public async Task<IActionResult> CreateApartment(CreateApartmentCommand createApartmentCommand)
        {
            var response = await Mediator.Send(createApartmentCommand);
            return Ok(response);
        }
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
        [HttpGet]
        public async Task<IActionResult> GetApartmentsByBlock(Guid? id, string? name, int currentPage = _defaultCurrentPage, int pageSize = _defaultCurrentPageSize)
        {
            if(!id.HasValue)
                id = Guid.Empty;
            if(string.IsNullOrEmpty(name))
                name = string.Empty;

            var response = await Mediator.Send(new GetListAllApartmentsByBlockQuery { BlockId = id.Value, BlockName = name, Page = currentPage, PageSize = pageSize });
            return Ok(response);    
        }
    }
}
