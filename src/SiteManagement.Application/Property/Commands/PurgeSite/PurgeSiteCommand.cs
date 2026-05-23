using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Commands.PurgeSite;

/// <summary>
/// Permanently (hard) deletes a site and cascades to its blocks + apartments.
/// Works on an already-archived site too. Admin-only — the explicit,
/// irreversible counterpart to the soft-delete <c>DeleteSiteCommand</c>.
/// </summary>
public sealed record PurgeSiteCommand(Guid SiteId) : ICommand;
