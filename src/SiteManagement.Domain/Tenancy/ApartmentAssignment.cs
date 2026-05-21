using SiteManagement.Domain.Shared;
using SiteManagement.Domain.Tenancy.Events;
using SiteManagement.Domain.Tenancy.Exceptions;
using SiteManagement.Domain.Tenancy.ValueObjects;

namespace SiteManagement.Domain.Tenancy;

/// <summary>
/// Aggregate root for the Tenancy bounded context: the historised link between
/// an apartment and the resident occupying it for a period, as owner or tenant.
/// The apartment and resident are referenced by id only — Property and
/// Residency are separate aggregates, kept consistent through domain events
/// rather than object references. A new assignment raises
/// <see cref="ResidentAssignedToApartment"/> (Property marks the unit occupied);
/// ending one raises <see cref="ResidentMovedOut"/> (Property frees it).
/// </summary>
public sealed class ApartmentAssignment : AggregateRoot<Guid>
{
    /// <summary>The assigned apartment (Property aggregate id).</summary>
    public Guid ApartmentId { get; private set; }

    /// <summary>The assigned resident (Residency aggregate id).</summary>
    public Guid ResidentId { get; private set; }

    /// <summary>Whether the resident holds the unit as owner or tenant.</summary>
    public TenantType TenantType { get; private set; }

    /// <summary>The effective date range; open while the resident still lives there.</summary>
    public AssignmentPeriod Period { get; private set; }

    /// <summary>True while the assignment has not ended.</summary>
    public bool IsActive => Period.IsActive;

    // EF Core materialisation ctor.
    private ApartmentAssignment()
    {
        Period = default!;
    }

    private ApartmentAssignment(Guid id, Guid apartmentId, Guid residentId, TenantType tenantType, AssignmentPeriod period)
        : base(id)
    {
        ApartmentId = apartmentId;
        ResidentId = residentId;
        TenantType = tenantType;
        Period = period;
    }

    /// <summary>
    /// Assigns a resident to an apartment from <paramref name="startDate"/>,
    /// raising <see cref="ResidentAssignedToApartment"/> so Property can mark
    /// the unit occupied.
    /// </summary>
    public static ApartmentAssignment Assign(
        Guid apartmentId,
        Guid residentId,
        TenantType tenantType,
        DateOnly startDate)
    {
        var assignment = new ApartmentAssignment(
            Guid.NewGuid(),
            apartmentId,
            residentId,
            tenantType,
            AssignmentPeriod.Open(startDate));

        assignment.RaiseDomainEvent(new ResidentAssignedToApartment(assignment.Id, apartmentId, residentId));
        return assignment;
    }

    /// <summary>
    /// Ends the assignment on <paramref name="endDate"/> (move-out), raising
    /// <see cref="ResidentMovedOut"/> so Property can free the unit.
    /// </summary>
    /// <exception cref="AssignmentAlreadyEndedException">Thrown when the assignment has already ended.</exception>
    /// <exception cref="InvalidAssignmentPeriodException">Thrown when the end date precedes the start date.</exception>
    public void End(DateOnly endDate)
    {
        if (!IsActive)
        {
            throw new AssignmentAlreadyEndedException(Id);
        }

        Period = Period.CloseOn(endDate);
        RaiseDomainEvent(new ResidentMovedOut(Id, ApartmentId, ResidentId));
    }
}
