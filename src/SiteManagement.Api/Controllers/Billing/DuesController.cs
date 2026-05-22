using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Billing.Commands.CloseDuesPeriod;
using SiteManagement.Application.Billing.Commands.DistributeDues;
using SiteManagement.Application.Billing.Commands.MarkDuesItemPaid;
using SiteManagement.Application.Billing.Commands.OpenDuesPeriod;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Billing.Queries.GetSiteDebtSummary;
using SiteManagement.Application.Billing.Queries.ListDuesPeriodItems;
using SiteManagement.Application.Billing.Queries.ListDuesPeriods;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Billing;

/// <summary>
/// Admin endpoints for the dues side of the Billing bounded context. A dues
/// period is opened with a fixed per-apartment amount, distributed into
/// per-occupant items, then closed. Read paths pivot on the owning site.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/dues")]
public class DuesController(ISender sender) : ControllerBase
{
    private const string DistributeRoute = "{duesPeriodId:guid}/distribute";
    private const string CloseRoute = "{duesPeriodId:guid}/close";
    private const string ItemsRoute = "{duesPeriodId:guid}/items";
    private const string PayItemRoute = "{duesPeriodId:guid}/items/{itemId:guid}/pay";
    private const string BySiteRoute = "sites/{siteId:guid}";
    private const string DebtSummaryRoute = "sites/{siteId:guid}/debt-summary";

    private readonly ISender _sender = sender;

    /// <summary>Opens an empty dues period for a site at a fixed per-apartment amount.</summary>
    [HttpPost]
    [ProducesResponseType<OpenDuesPeriodResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OpenDuesPeriodResult>> Open(
        [FromBody] OpenDuesPeriodRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new OpenDuesPeriodCommand(body.SiteId, body.Year, body.Month, body.PerApartmentAmount), ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Distributes dues items to every occupied apartment in the period's site.</summary>
    [HttpPost(DistributeRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Distribute(Guid duesPeriodId, CancellationToken ct)
    {
        await _sender.Send(new DistributeDuesCommand(duesPeriodId), ct);
        return NoContent();
    }

    /// <summary>Closes a dues period (no further items can be added).</summary>
    [HttpPost(CloseRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Close(Guid duesPeriodId, CancellationToken ct)
    {
        await _sender.Send(new CloseDuesPeriodCommand(duesPeriodId), ct);
        return NoContent();
    }

    /// <summary>Lists a site's dues periods (most recent month first).</summary>
    [HttpGet(BySiteRoute)]
    [ProducesResponseType<IReadOnlyList<DuesPeriodListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DuesPeriodListItemDto>>> ListForSite(Guid siteId, CancellationToken ct)
        => Ok(await _sender.Send(new ListDuesPeriodsQuery(siteId), ct));

    /// <summary>Marks one dues item paid.</summary>
    [HttpPost(PayItemRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayItem(Guid duesPeriodId, Guid itemId, CancellationToken ct)
    {
        await _sender.Send(new MarkDuesItemPaidCommand(duesPeriodId, itemId), ct);
        return NoContent();
    }

    /// <summary>Lists the distributed per-apartment items of one dues period.</summary>
    [HttpGet(ItemsRoute)]
    [ProducesResponseType<IReadOnlyList<PeriodItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PeriodItemDto>>> ListItems(Guid duesPeriodId, CancellationToken ct)
        => Ok(await _sender.Send(new ListDuesPeriodItemsQuery(duesPeriodId), ct));

    /// <summary>Returns the accrued / collected / outstanding totals for a site.</summary>
    [HttpGet(DebtSummaryRoute)]
    [ProducesResponseType<SiteDebtSummaryDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SiteDebtSummaryDto>> GetDebtSummary(Guid siteId, CancellationToken ct)
        => Ok(await _sender.Send(new GetSiteDebtSummaryQuery(siteId), ct));
}
