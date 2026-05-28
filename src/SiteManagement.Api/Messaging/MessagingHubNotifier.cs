using Microsoft.AspNetCore.SignalR;
using SiteManagement.Application.Messaging.Notifications;

namespace SiteManagement.Api.Messaging;

/// <summary>
/// SignalR adapter for <see cref="IMessagingNotifier"/>. Fans the payload out
/// to the right group (admins or a single resident) using the payload's
/// <see cref="IMessagingNotification.EventName"/> as the SignalR method name.
/// </summary>
public sealed class MessagingHubNotifier(IHubContext<MessagingHub> hub) : IMessagingNotifier
{
    private readonly IHubContext<MessagingHub> _hub = hub;

    /// <inheritdoc />
    public Task NotifyAdminsAsync(IMessagingNotification payload, CancellationToken cancellationToken = default)
        => _hub.Clients.Group(MessagingGroups.Admins).SendAsync(payload.EventName, payload, cancellationToken);

    /// <inheritdoc />
    public Task NotifyResidentAsync(Guid residentId, IMessagingNotification payload, CancellationToken cancellationToken = default)
        => _hub.Clients.Group(MessagingGroups.ForResident(residentId)).SendAsync(payload.EventName, payload, cancellationToken);
}
