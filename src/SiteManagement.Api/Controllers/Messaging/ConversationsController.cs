using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Messaging.Commands;
using SiteManagement.Application.Messaging.Commands.MarkConversationRead;
using SiteManagement.Application.Messaging.Commands.ReplyToConversation;
using SiteManagement.Application.Messaging.Commands.StartConversation;
using SiteManagement.Application.Messaging.Queries;
using SiteManagement.Application.Messaging.Queries.GetConversationMessages;
using SiteManagement.Application.Messaging.Queries.ListConversations;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Messaging;

/// <summary>
/// Admin side of the Messaging context: open a thread with a resident, list the
/// inbox, read a thread's messages, reply, and mark read. Role is gated here and
/// again by the authorization pipeline.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/conversations")]
public class ConversationsController(ISender sender) : ControllerBase
{
    private const string MessagesRoute = "{conversationId:guid}/messages";
    private const string ReadRoute = "{conversationId:guid}/read";

    private readonly ISender _sender = sender;

    /// <summary>Opens a new conversation with a resident and posts the first message.</summary>
    [HttpPost]
    [ProducesResponseType<ConversationCreatedResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConversationCreatedResult>> Start(
        [FromBody] StartConversationRequest body,
        CancellationToken ct)
    {
        var result = await _sender.Send(new StartConversationCommand(body.ResidentId, body.Subject, body.Body), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Lists every conversation (most recently active first).</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ConversationListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ConversationListItemDto>>> List(CancellationToken ct)
        => Ok(await _sender.Send(new ListConversationsQuery(), ct));

    /// <summary>Lists one conversation's messages in send order.</summary>
    [HttpGet(MessagesRoute)]
    [ProducesResponseType<IReadOnlyList<MessageDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> Messages(Guid conversationId, CancellationToken ct)
        => Ok(await _sender.Send(new GetConversationMessagesQuery(conversationId), ct));

    /// <summary>Appends an admin reply to the conversation.</summary>
    [HttpPost(MessagesRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reply(Guid conversationId, [FromBody] MessageBodyRequest body, CancellationToken ct)
    {
        await _sender.Send(new ReplyToConversationCommand(conversationId, body.Body), ct);
        return NoContent();
    }

    /// <summary>Marks the resident's messages in the thread as read.</summary>
    [HttpPost(ReadRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(Guid conversationId, CancellationToken ct)
    {
        await _sender.Send(new MarkConversationReadCommand(conversationId), ct);
        return NoContent();
    }
}

/// <summary>Body for opening a conversation with a resident.</summary>
public sealed record StartConversationRequest(Guid ResidentId, string Subject, string Body);

/// <summary>Body carrying a single message's text.</summary>
public sealed record MessageBodyRequest(string Body);
