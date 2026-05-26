using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Commands.CreateSite;

/// <summary>Creates a brand-new site with no blocks yet. Admin-only command.</summary>
public sealed record CreateSiteCommand(string Name, string Address) : ICommand<CreateSiteResult>, IAdminRequest;

/// <summary>Result carrying the new site's identifier.</summary>
public sealed record CreateSiteResult(Guid SiteId);
