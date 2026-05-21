namespace SiteManagement.Application.Tenancy.Queries;

/// <summary>
/// The active occupant of an apartment, as shown on the site detail page.
/// </summary>
public sealed record ApartmentOccupantDto(
    Guid AssignmentId,
    Guid ApartmentId,
    Guid ResidentId,
    string ResidentFullName,
    string TenantType,
    DateOnly StartDate);

/// <summary>One assignment row in a resident's tenancy history.</summary>
public sealed record ResidentAssignmentDto(
    Guid AssignmentId,
    Guid ApartmentId,
    string TenantType,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsActive);
