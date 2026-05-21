using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Billing.Queries.ListResidentBills;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Residents;

/// <summary>
/// Admin read endpoint for a single resident's billing lines, nested under the
/// resident resource. Cross-context Billing query — kept on its own controller
/// so the Residency controller stays focused on the resident aggregate.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/residents/{{residentId:guid}}/bills")]
public class ResidentBillsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>Returns every outstanding + paid line owed by the resident.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ResidentBillDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResidentBillDto>>> List(Guid residentId, CancellationToken ct)
        => Ok(await _sender.Send(new ListResidentBillsQuery(residentId), ct));
}
