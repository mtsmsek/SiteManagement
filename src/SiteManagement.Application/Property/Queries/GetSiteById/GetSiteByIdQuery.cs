using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Queries.GetSiteById;

/// <summary>Returns the full site detail projection. Admin-only.</summary>
public sealed record GetSiteByIdQuery(Guid SiteId) : IQuery<SiteDetailsDto>;
