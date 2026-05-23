using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Property.Commands.AddBlock;
using SiteManagement.Application.Property.Commands.CreateSite;
using SiteManagement.Application.Property.Commands.DeleteSite;
using SiteManagement.Application.Property.Commands.PurgeSite;
using SiteManagement.Application.Property.Queries;
using SiteManagement.Application.Property.Queries.GetSiteById;
using SiteManagement.Application.Property.Queries.ListSites;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Sites;

/// <summary>
/// Admin endpoints for the Property bounded context. Sites + blocks live
/// here; apartment-level mutations sit on the separate
/// <see cref="Apartments.ApartmentsController"/> because the URL hierarchy
/// is flatter (apartments are globally identified by id).
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/sites")]
public class SitesController(ISender sender) : ControllerBase
{
    private const string ByIdRoute = "{siteId:guid}";
    private const string BlocksRoute = "{siteId:guid}/blocks";
    private const string PurgeRoute = "{siteId:guid}/permanent";

    private readonly ISender _sender = sender;

    /// <summary>Returns every site with block + apartment totals.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SiteListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SiteListItemDto>>> List(CancellationToken ct)
        => Ok(await _sender.Send(new ListSitesQuery(), ct));

    /// <summary>Returns the full site detail (blocks + apartments).</summary>
    [HttpGet(ByIdRoute)]
    [ProducesResponseType<SiteDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SiteDetailsDto>> GetById(Guid siteId, CancellationToken ct)
        => Ok(await _sender.Send(new GetSiteByIdQuery(siteId), ct));

    /// <summary>Creates a new site.</summary>
    [HttpPost]
    [ProducesResponseType<CreateSiteResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSiteResponse>> Create(
        [FromBody] CreateSiteRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(new CreateSiteCommand(body.Name, body.Address), ct);
        return CreatedAtAction(nameof(GetById), new { siteId = result.SiteId }, new CreateSiteResponse(result.SiteId));
    }

    /// <summary>Adds a block to an existing site.</summary>
    [HttpPost(BlocksRoute)]
    [ProducesResponseType<AddBlockResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AddBlockResponse>> AddBlock(
        Guid siteId,
        [FromBody] AddBlockRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(new AddBlockCommand(siteId, body.Name), ct);
        return CreatedAtAction(nameof(GetById), new { siteId }, new AddBlockResponse(result.BlockId));
    }

    /// <summary>Soft-deletes (archives) a site; it disappears from reads but its data is kept.</summary>
    [HttpDelete(ByIdRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid siteId, CancellationToken ct)
    {
        await _sender.Send(new DeleteSiteCommand(siteId), ct);
        return NoContent();
    }

    /// <summary>Permanently deletes a site (and its blocks + apartments). Irreversible.</summary>
    [HttpDelete(PurgeRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Purge(Guid siteId, CancellationToken ct)
    {
        await _sender.Send(new PurgeSiteCommand(siteId), ct);
        return NoContent();
    }
}
