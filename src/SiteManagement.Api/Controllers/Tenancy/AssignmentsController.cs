using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Tenancy.Commands.AssignResident;
using SiteManagement.Application.Tenancy.Commands.EndAssignment;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Application.Tenancy.Queries.GetResidentAssignments;
using SiteManagement.Application.Tenancy.Queries.GetSiteOccupants;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Tenancy;

/// <summary>
/// Admin endpoints for the Tenancy bounded context. Assignments link a
/// resident to an apartment for a date range; the URL hierarchy is flat
/// because assignments carry globally unique ids and the read paths pivot
/// on either a site or a resident.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/assignments")]
public class AssignmentsController(ISender sender) : ControllerBase
{
    private const string EndRoute = "{assignmentId:guid}/end";
    private const string SiteOccupantsRoute = "sites/{siteId:guid}/occupants";
    private const string ResidentAssignmentsRoute = "residents/{residentId:guid}";

    private readonly ISender _sender = sender;

    /// <summary>Assigns a resident to an apartment as owner or tenant.</summary>
    [HttpPost]
    [ProducesResponseType<AssignResidentResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AssignResidentResult>> Assign(
        [FromBody] AssignResidentRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new AssignResidentCommand(body.ApartmentId, body.ResidentId, body.TenantType, body.StartDate), ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Ends an apartment assignment (move-out) on the given date.</summary>
    [HttpPost(EndRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> End(
        Guid assignmentId,
        [FromBody] EndAssignmentRequest body,
        CancellationToken ct)
    {
        await _sender.Send(new EndAssignmentCommand(assignmentId, body.EndDate), ct);
        return NoContent();
    }

    /// <summary>Returns the active occupants of every apartment in a site.</summary>
    [HttpGet(SiteOccupantsRoute)]
    [ProducesResponseType<IReadOnlyList<ApartmentOccupantDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ApartmentOccupantDto>>> GetSiteOccupants(Guid siteId, CancellationToken ct)
        => Ok(await _sender.Send(new GetSiteOccupantsQuery(siteId), ct));

    /// <summary>Returns a resident's assignment history (most recent first).</summary>
    [HttpGet(ResidentAssignmentsRoute)]
    [ProducesResponseType<IReadOnlyList<ResidentAssignmentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResidentAssignmentDto>>> GetResidentAssignments(Guid residentId, CancellationToken ct)
        => Ok(await _sender.Send(new GetResidentAssignmentsQuery(residentId), ct));
}
