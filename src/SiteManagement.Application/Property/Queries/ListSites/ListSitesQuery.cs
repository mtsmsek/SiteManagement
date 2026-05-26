using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Queries.ListSites;

/// <summary>Returns one row per site with block + apartment counts. Admin-only.</summary>
public sealed record ListSitesQuery : IQuery<IReadOnlyList<SiteListItemDto>>, IAdminRequest;
