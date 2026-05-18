using MediatR;

namespace SiteManagement.Application.Property.Queries.GetSiteById;

/// <summary>Returns the full site detail projection. Admin-only.</summary>
public sealed record GetSiteByIdQuery(Guid SiteId) : IRequest<SiteDetailsDto>;
