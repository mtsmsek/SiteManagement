using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Messaging;

/// <summary>
/// Real-time push channel for admin ↔ resident conversations. The hub itself
/// exposes no client-callable methods — sends are server-to-client only;
/// posting messages stays on the HTTP command path so validation, ownership,
/// and the transaction pipeline run uniformly. On connect, each authenticated
/// client joins the group its role maps to (admins → <see cref="MessagingGroups.Admins"/>,
/// resident → <see cref="MessagingGroups.ForResident"/> keyed by their token's
/// <c>resident_id</c> claim), so a resident is structurally unable to subscribe
/// to another resident's stream.
/// </summary>
[Authorize]
public sealed class MessagingHub : Hub
{
    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        if (user is null)
        {
            Context.Abort();
            return;
        }

        if (user.IsInRole(Roles.Admin))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, MessagingGroups.Admins);
        }
        else if (user.IsInRole(Roles.Resident))
        {
            var residentClaim = user.FindFirst(AuthClaims.ResidentId)?.Value;
            if (Guid.TryParse(residentClaim, out var residentId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, MessagingGroups.ForResident(residentId));
            }
        }

        await base.OnConnectedAsync();
    }
}
