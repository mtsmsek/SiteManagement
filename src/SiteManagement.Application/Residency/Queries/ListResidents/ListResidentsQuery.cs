using MediatR;

namespace SiteManagement.Application.Residency.Queries.ListResidents;

/// <summary>Lists every resident with the columns the admin list page renders. Admin-only.</summary>
public sealed record ListResidentsQuery : IRequest<IReadOnlyList<ResidentListItemDto>>;
