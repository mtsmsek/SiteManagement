using MediatR;

namespace SiteManagement.Application.Property.Commands.DeleteSite;

/// <summary>
/// Deletes a site, cascading to its blocks + apartments. Admin-only.
/// </summary>
public sealed record DeleteSiteCommand(Guid SiteId) : IRequest;
