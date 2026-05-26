using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Application.Tenancy.Commands.AssignResident;

/// <summary>
/// Assigns a resident to an apartment as owner or tenant, starting on
/// <paramref name="StartDate"/>. Creating the assignment raises
/// <c>ResidentAssignedToApartment</c>, which a Property-side handler reacts to
/// by marking the apartment occupied — both writes commit in one transaction.
/// Admin-only command.
/// </summary>
public sealed record AssignResidentCommand(
    Guid ApartmentId,
    Guid ResidentId,
    TenantType TenantType,
    DateOnly StartDate) : ICommand<AssignResidentResult>, IAdminRequest;

/// <summary>Result carrying the new assignment's identifier.</summary>
public sealed record AssignResidentResult(Guid AssignmentId);
