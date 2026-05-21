using MediatR;

namespace SiteManagement.Application.Tenancy.Queries.GetResidentAssignments;

/// <summary>Returns a resident's assignment history (most recent first). Admin-only.</summary>
public sealed record GetResidentAssignmentsQuery(Guid ResidentId) : IRequest<IReadOnlyList<ResidentAssignmentDto>>;
