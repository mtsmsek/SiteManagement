namespace SiteManagement.Api.Messaging;

/// <summary>
/// SignalR group naming for the messaging hub. Centralised so the hub side
/// (which adds connections to groups) and the notifier side (which targets
/// them) cannot drift apart. No magic strings sprinkled at the call sites.
/// </summary>
public static class MessagingGroups
{
    /// <summary>Group every authenticated admin client joins on connect.</summary>
    public const string Admins = "messaging:admins";

    /// <summary>Group a resident's authenticated clients join on connect, keyed by their <c>Resident.Id</c>.</summary>
    public static string ForResident(Guid residentId) => $"messaging:resident:{residentId:N}";
}
