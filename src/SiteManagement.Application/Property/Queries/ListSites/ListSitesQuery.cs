using MediatR;

namespace SiteManagement.Application.Property.Queries.ListSites;

/// <summary>Returns one row per site with block + apartment counts. Admin-only.</summary>
public sealed record ListSitesQuery : IRequest<IReadOnlyList<SiteListItemDto>>;
