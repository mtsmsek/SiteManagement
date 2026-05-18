using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Api.Controllers.Sites;
using SiteManagement.Application.Property.Commands.AddApartment;
using SiteManagement.Application.Property.Commands.MarkApartmentEmpty;
using SiteManagement.Application.Property.Commands.MarkApartmentOccupied;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Apartments;

/// <summary>
/// Admin endpoints for apartments. URL hierarchy is flat: apartments and
/// blocks have globally unique ids, so commands take the leaf id and the
/// command handler resolves the owning aggregate server-side.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route(ApiConstants.RoutePrefix)]
public class ApartmentsController(ISender sender) : ControllerBase
{
    private const string AddApartmentRoute = "blocks/{blockId:guid}/apartments";
    private const string OccupyRoute = "apartments/{apartmentId:guid}/occupy";
    private const string VacateRoute = "apartments/{apartmentId:guid}/vacate";

    private readonly ISender _sender = sender;

    /// <summary>Adds an apartment to a block.</summary>
    [HttpPost(AddApartmentRoute)]
    [ProducesResponseType<AddApartmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AddApartmentResponse>> Add(
        Guid blockId,
        [FromBody] AddApartmentRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new AddApartmentCommand(blockId, body.Number, body.Floor, body.Type), ct);

        return StatusCode(StatusCodes.Status201Created, new AddApartmentResponse(result.ApartmentId));
    }

    /// <summary>Marks an apartment as occupied (Empty -> Occupied).</summary>
    [HttpPost(OccupyRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Occupy(Guid apartmentId, CancellationToken ct)
    {
        await _sender.Send(new MarkApartmentOccupiedCommand(apartmentId), ct);
        return NoContent();
    }

    /// <summary>Marks an apartment as empty (Occupied -> Empty).</summary>
    [HttpPost(VacateRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Vacate(Guid apartmentId, CancellationToken ct)
    {
        await _sender.Send(new MarkApartmentEmptyCommand(apartmentId), ct);
        return NoContent();
    }
}
