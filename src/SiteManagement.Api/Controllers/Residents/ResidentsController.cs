using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Residency.Commands.AddVehicle;
using SiteManagement.Application.Residency.Commands.RegisterResident;
using SiteManagement.Application.Residency.Commands.RemoveVehicle;
using SiteManagement.Application.Residency.Commands.UpdateContactInfo;
using SiteManagement.Application.Residency.Queries;
using SiteManagement.Application.Residency.Queries.GetResidentById;
using SiteManagement.Application.Residency.Queries.ListResidents;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Residents;

/// <summary>
/// Admin endpoints for the Residency bounded context. Resident-level
/// self-service endpoints (own profile, own dues, own invoices) arrive
/// in W3-W5 under a separate <c>/api/me</c> controller; this one is
/// admin-only.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/residents")]
public class ResidentsController(ISender sender) : ControllerBase
{
    private const string ByIdRoute = "{residentId:guid}";
    private const string ContactRoute = "{residentId:guid}/contact";
    private const string VehiclesRoute = "{residentId:guid}/vehicles";
    private const string VehicleByPlateRoute = "{residentId:guid}/vehicles/{plate}";

    private readonly ISender _sender = sender;

    /// <summary>Returns every resident with the columns the list page renders.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ResidentListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResidentListItemDto>>> List(CancellationToken ct)
        => Ok(await _sender.Send(new ListResidentsQuery(), ct));

    /// <summary>Returns the full resident detail.</summary>
    [HttpGet(ByIdRoute)]
    [ProducesResponseType<ResidentDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResidentDetailsDto>> GetById(Guid residentId, CancellationToken ct)
        => Ok(await _sender.Send(new GetResidentByIdQuery(residentId), ct));

    /// <summary>Creates a new resident + linked AppUser; emails the temporary password.</summary>
    [HttpPost]
    [ProducesResponseType<RegisterResidentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResidentResponse>> Register(
        [FromBody] RegisterResidentRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RegisterResidentCommand(body.TcNo, body.FirstName, body.LastName, body.Email, body.Phone),
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new { residentId = result.ResidentId },
            new RegisterResidentResponse(result.ResidentId));
    }

    /// <summary>Replaces the resident's email + phone.</summary>
    [HttpPut(ContactRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContact(
        Guid residentId,
        [FromBody] UpdateContactRequest body,
        CancellationToken ct)
    {
        await _sender.Send(new UpdateContactInfoCommand(residentId, body.Email, body.Phone), ct);
        return NoContent();
    }

    /// <summary>Registers a vehicle on the resident.</summary>
    [HttpPost(VehiclesRoute)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddVehicle(
        Guid residentId,
        [FromBody] AddVehicleRequest body,
        CancellationToken ct)
    {
        await _sender.Send(new AddVehicleCommand(residentId, body.Plate, body.Note), ct);
        return CreatedAtAction(nameof(GetById), new { residentId }, null);
    }

    /// <summary>Removes a vehicle (by plate) from the resident.</summary>
    [HttpDelete(VehicleByPlateRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveVehicle(Guid residentId, string plate, CancellationToken ct)
    {
        await _sender.Send(new RemoveVehicleCommand(residentId, plate), ct);
        return NoContent();
    }
}
