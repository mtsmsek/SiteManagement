using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Tenancy.Queries.GetSiteOccupants;

/// <summary>Returns the active occupants of every apartment in a site. Admin-only.</summary>
public sealed record GetSiteOccupantsQuery(Guid SiteId) : IQuery<IReadOnlyList<ApartmentOccupantDto>>, IAdminRequest;
