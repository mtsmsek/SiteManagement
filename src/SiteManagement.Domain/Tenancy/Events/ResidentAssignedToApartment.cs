using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Tenancy.Events;

/// <summary>
/// Raised when a resident is assigned to an apartment. A Property-side handler
/// reacts by marking the apartment occupied, keeping the two aggregates
/// consistent without a direct object reference between them.
/// </summary>
/// <param name="AssignmentId">The new assignment's id.</param>
/// <param name="ApartmentId">The apartment that was assigned.</param>
/// <param name="ResidentId">The resident who was assigned.</param>
public sealed record ResidentAssignedToApartment(
    Guid AssignmentId,
    Guid ApartmentId,
    Guid ResidentId) : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
