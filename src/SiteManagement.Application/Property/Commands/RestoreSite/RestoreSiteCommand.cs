using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Commands.RestoreSite;

/// <summary>
/// Restores a soft-deleted (archived) site, bringing it and its data back into
/// reads. Admin-only. The undo to <c>DeleteSiteCommand</c>.
/// </summary>
public sealed record RestoreSiteCommand(Guid SiteId) : ICommand, IAdminRequest;
