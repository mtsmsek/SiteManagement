using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SiteManagement.Api.Configuration;
using SiteManagement.Api.Controllers.Billing;
using SiteManagement.Api.Controllers.Messaging;
using SiteManagement.Application.Billing.Commands.PayMyDuesItem;
using SiteManagement.Application.Billing.Commands.PayMyUtilityItem;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Billing.Queries.GetMyBills;
using SiteManagement.Application.Messaging.Commands;
using SiteManagement.Application.Messaging.Commands.MarkMyConversationRead;
using SiteManagement.Application.Messaging.Commands.ReplyToMyConversation;
using SiteManagement.Application.Messaging.Commands.StartMyConversation;
using SiteManagement.Application.Messaging.Queries;
using SiteManagement.Application.Messaging.Queries.GetMyConversationMessages;
using SiteManagement.Application.Messaging.Queries.ListMyConversations;
using SiteManagement.Application.Reports.Queries;
using SiteManagement.Application.Reports.Queries.GetMyDashboard;
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
    private const string DashboardRoute = "dashboard";
    private const string BillsRoute = "bills";
    private const string PayDuesRoute = "dues/{duesPeriodId:guid}/items/{itemId:guid}/pay-by-card";
    private const string PayUtilityRoute = "utility-bills/{utilityBillPeriodId:guid}/items/{itemId:guid}/pay-by-card";
    private const string ConversationsRoute = "conversations";
    private const string ConversationMessagesRoute = "conversations/{conversationId:guid}/messages";
    private const string ConversationReadRoute = "conversations/{conversationId:guid}/read";

    private readonly ISender _sender = sender;

    /// <summary>The current resident's portal summary (outstanding, credit, unread messages).</summary>
    [HttpGet(DashboardRoute)]
    [ProducesResponseType<ResidentDashboardDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ResidentDashboardDto>> Dashboard(CancellationToken ct)
        => Ok(await _sender.Send(new GetMyDashboardQuery(), ct));

    /// <summary>Lists the current resident's own outstanding + paid bills.</summary>
    [HttpGet(BillsRoute)]
    [ProducesResponseType<IReadOnlyList<ResidentBillDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResidentBillDto>>> ListMyBills(CancellationToken ct)
        => Ok(await _sender.Send(new GetMyBillsQuery(), ct));

    /// <summary>Pays one of the resident's own dues items by credit card.</summary>
    [HttpPost(PayDuesRoute)]
    [EnableRateLimiting(RateLimitingExtensions.PayByCardPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    [EnableRateLimiting(RateLimitingExtensions.PayByCardPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

    /// <summary>Lists the current resident's own conversations.</summary>
    [HttpGet(ConversationsRoute)]
    [ProducesResponseType<IReadOnlyList<ConversationListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ConversationListItemDto>>> ListMyConversations(CancellationToken ct)
        => Ok(await _sender.Send(new ListMyConversationsQuery(), ct));

    /// <summary>Opens a new conversation for the resident with a first message.</summary>
    [HttpPost(ConversationsRoute)]
    [ProducesResponseType<ConversationCreatedResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConversationCreatedResult>> StartMyConversation(
        [FromBody] StartMyConversationRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(new StartMyConversationCommand(body.Subject, body.Body), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Lists the messages of one of the resident's own conversations.</summary>
    [HttpGet(ConversationMessagesRoute)]
    [ProducesResponseType<IReadOnlyList<MessageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> MyConversationMessages(Guid conversationId, CancellationToken ct)
        => Ok(await _sender.Send(new GetMyConversationMessagesQuery(conversationId), ct));

    /// <summary>Appends the resident's reply to one of their own conversations.</summary>
    [HttpPost(ConversationMessagesRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReplyToMyConversation(
        Guid conversationId,
        [FromBody] MessageBodyRequest body,
        CancellationToken ct)
    {
        await _sender.Send(new ReplyToMyConversationCommand(conversationId, body.Body), ct);
        return NoContent();
    }

    /// <summary>Marks the admin's messages in the resident's own thread as read.</summary>
    [HttpPost(ConversationReadRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkMyConversationRead(Guid conversationId, CancellationToken ct)
    {
        await _sender.Send(new MarkMyConversationReadCommand(conversationId), ct);
        return NoContent();
    }
}

/// <summary>Body for a resident opening their own conversation.</summary>
public sealed record StartMyConversationRequest(string Subject, string Body);
