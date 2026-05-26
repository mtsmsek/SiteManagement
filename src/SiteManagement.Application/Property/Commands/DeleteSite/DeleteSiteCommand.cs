using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Commands.DeleteSite;

/// <summary>
/// Soft-deletes (archives) a site so it disappears from reads while its data is
/// kept. Admin-only. See <c>PurgeSiteCommand</c> for the permanent hard delete.
/// </summary>
public sealed record DeleteSiteCommand(Guid SiteId) : ICommand, IAdminRequest;
