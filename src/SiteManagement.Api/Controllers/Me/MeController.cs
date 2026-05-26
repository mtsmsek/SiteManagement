using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Api.Controllers.Billing;
using SiteManagement.Application.Billing.Commands.PayMyDuesItem;
using SiteManagement.Application.Billing.Commands.PayMyUtilityItem;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Billing.Queries.GetMyBills;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Me;

/// <summary>
/// Resident self-service endpoints, always scoped to the authenticated caller —
/// the resident id is never a route parameter, so a resident can only ever read
/// and pay their own bills. Role is gated here (defense in depth) and again by
/// the authorization pipeline; ownership of a paid item is enforced by the
/// pipeline's resource-ownership behavior, not in any handler.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Resident)]
[Route($"{ApiConstants.RoutePrefix}/me")]
public class MeController(ISender sender) : ControllerBase
{
    private const string BillsRoute = "bills";
    private const string PayDuesRoute = "dues/{duesPeriodId:guid}/items/{itemId:guid}/pay-by-card";
    private const string PayUtilityRoute = "utility-bills/{utilityBillPeriodId:guid}/items/{itemId:guid}/pay-by-card";

    private readonly ISender _sender = sender;

    /// <summary>Lists the current resident's own outstanding + paid bills.</summary>
    [HttpGet(BillsRoute)]
    [ProducesResponseType<IReadOnlyList<ResidentBillDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResidentBillDto>>> ListMyBills(CancellationToken ct)
        => Ok(await _sender.Send(new GetMyBillsQuery(), ct));

    /// <summary>Pays one of the resident's own dues items by credit card.</summary>
    [HttpPost(PayDuesRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayDuesItem(
        Guid duesPeriodId,
        Guid itemId,
        [FromBody] PayByCardRequest body,
        CancellationToken ct)
    {
        await _sender.Send(
            new PayMyDuesItemCommand(
                duesPeriodId, itemId, body.CardNumber, body.Cvv, body.ExpiryYear, body.ExpiryMonth),
            ct);
        return NoContent();
    }

    /// <summary>Pays one of the resident's own utility bill items by credit card.</summary>
    [HttpPost(PayUtilityRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayUtilityItem(
        Guid utilityBillPeriodId,
        Guid itemId,
        [FromBody] PayByCardRequest body,
        CancellationToken ct)
    {
        await _sender.Send(
            new PayMyUtilityItemCommand(
                utilityBillPeriodId, itemId, body.CardNumber, body.Cvv, body.ExpiryYear, body.ExpiryMonth),
            ct);
        return NoContent();
    }
}
