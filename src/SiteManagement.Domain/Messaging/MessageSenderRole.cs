namespace SiteManagement.Domain.Messaging;

/// <summary>
/// Which side of an admin ↔ resident conversation authored a message. Drives
/// the read model (a reader marks the <em>other</em> side's messages read) and
/// is stored as a string so the column stays human-readable.
/// </summary>
public enum MessageSenderRole
{
    /// <summary>Sent by site management (an admin).</summary>
    Admin = 0,

    /// <summary>Sent by the resident.</summary>
    Resident = 1,
}
