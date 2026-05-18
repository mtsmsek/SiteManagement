using MediatR;

namespace SiteManagement.Application.Property.Commands.CreateSite;

/// <summary>Creates a brand-new site with no blocks yet. Admin-only command.</summary>
public sealed record CreateSiteCommand(string Name, string Address) : IRequest<CreateSiteResult>;

/// <summary>Result carrying the new site's identifier.</summary>
public sealed record CreateSiteResult(Guid SiteId);
