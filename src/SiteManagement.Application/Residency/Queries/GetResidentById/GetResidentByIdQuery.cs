using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Residency.Queries.GetResidentById;

/// <summary>Loads the resident detail projection (vehicles included). Admin-only.</summary>
public sealed record GetResidentByIdQuery(Guid ResidentId) : IQuery<ResidentDetailsDto>, IAdminRequest;
