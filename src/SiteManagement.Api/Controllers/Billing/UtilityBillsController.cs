using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Billing.Commands.ChangeUtilityBillAmount;
using SiteManagement.Application.Billing.Commands.CloseUtilityBillPeriod;
using SiteManagement.Application.Billing.Commands.DistributeUtilityBill;
using SiteManagement.Application.Billing.Commands.MarkUtilityBillItemPaid;
using SiteManagement.Application.Billing.Commands.OpenUtilityBillPeriod;
using SiteManagement.Application.Billing.Commands.PayUtilityItem;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Billing.Queries.ListUtilityBillPeriodItems;
using SiteManagement.Application.Billing.Queries.ListUtilityBillPeriods;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Billing;

/// <summary>
/// Admin endpoints for the utility-bill side of the Billing bounded context. A
/// utility bill period is opened with a total amount for a utility type,
/// distributed across occupants, then closed. Reads pivot on the owning site.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/utility-bills")]
public class UtilityBillsController(ISender sender) : ControllerBase
{
    private const string ChangeAmountRoute = "{utilityBillPeriodId:guid}";
    private const string DistributeRoute = "{utilityBillPeriodId:guid}/distribute";
    private const string CloseRoute = "{utilityBillPeriodId:guid}/close";
    private const string ItemsRoute = "{utilityBillPeriodId:guid}/items";
    private const string PayItemRoute = "{utilityBillPeriodId:guid}/items/{itemId:guid}/pay";
    private const string PayItemByCardRoute = "{utilityBillPeriodId:guid}/items/{itemId:guid}/pay-by-card";
    private const string BySiteRoute = "sites/{siteId:guid}";

    private readonly ISender _sender = sender;

    /// <summary>Opens an empty utility bill period for a site at a fixed total amount.</summary>
    [HttpPost]
    [ProducesResponseType<OpenUtilityBillPeriodResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OpenUtilityBillPeriodResult>> Open(
        [FromBody] OpenUtilityBillRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new OpenUtilityBillPeriodCommand(body.SiteId, body.Year, body.Month, body.UtilityType, body.TotalAmount), ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Corrects an open utility bill period's total, re-splitting it across the items.</summary>
    [HttpPut(ChangeAmountRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeAmount(
        Guid utilityBillPeriodId,
        [FromBody] ChangeUtilityBillAmountRequest body,
        CancellationToken ct)
    {
        await _sender.Send(new ChangeUtilityBillAmountCommand(utilityBillPeriodId, body.TotalAmount), ct);
        return NoContent();
    }

    /// <summary>Distributes the total across every occupied apartment in the period's site.</summary>
    [HttpPost(DistributeRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Distribute(Guid utilityBillPeriodId, CancellationToken ct)
    {
        await _sender.Send(new DistributeUtilityBillCommand(utilityBillPeriodId), ct);
        return NoContent();
    }

    /// <summary>Closes a utility bill period (no further items can be added).</summary>
    [HttpPost(CloseRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Close(Guid utilityBillPeriodId, CancellationToken ct)
    {
        await _sender.Send(new CloseUtilityBillPeriodCommand(utilityBillPeriodId), ct);
        return NoContent();
    }

    /// <summary>Marks one utility bill item paid.</summary>
    [HttpPost(PayItemRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayItem(Guid utilityBillPeriodId, Guid itemId, CancellationToken ct)
    {
        await _sender.Send(new MarkUtilityBillItemPaidCommand(utilityBillPeriodId, itemId), ct);
        return NoContent();
    }

    /// <summary>Pays one utility bill item by credit card via the payment gateway.</summary>
    [HttpPost(PayItemByCardRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayItemByCard(
        Guid utilityBillPeriodId,
        Guid itemId,
        [FromBody] PayByCardRequest body,
        CancellationToken ct)
    {
        await _sender.Send(
            new PayUtilityItemCommand(
                utilityBillPeriodId, itemId, body.CardNumber, body.Cvv, body.ExpiryYear, body.ExpiryMonth),
            ct);
        return NoContent();
    }

    /// <summary>Lists the distributed per-apartment items of one utility bill period.</summary>
    [HttpGet(ItemsRoute)]
    [ProducesResponseType<IReadOnlyList<PeriodItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PeriodItemDto>>> ListItems(Guid utilityBillPeriodId, CancellationToken ct)
        => Ok(await _sender.Send(new ListUtilityBillPeriodItemsQuery(utilityBillPeriodId), ct));

    /// <summary>Lists a site's utility bill periods (most recent month first).</summary>
    [HttpGet(BySiteRoute)]
    [ProducesResponseType<IReadOnlyList<UtilityBillPeriodListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UtilityBillPeriodListItemDto>>> ListForSite(Guid siteId, CancellationToken ct)
        => Ok(await _sender.Send(new ListUtilityBillPeriodsQuery(siteId), ct));
}
