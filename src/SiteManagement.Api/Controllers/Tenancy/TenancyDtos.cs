using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Api.Controllers.Tenancy;

/// <summary>Request body for <c>POST /api/assignments</c>.</summary>
public sealed record AssignResidentRequest(
    Guid ApartmentId,
    Guid ResidentId,
    TenantType TenantType,
    DateOnly StartDate);

/// <summary>Request body for <c>POST /api/assignments/{assignmentId}/end</c>.</summary>
public sealed record EndAssignmentRequest(DateOnly EndDate);
