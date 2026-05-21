using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Tenancy.Events;

/// <summary>
/// Raised when an apartment assignment ends (the resident moved out). A
/// Property-side handler reacts by marking the apartment empty again.
/// </summary>
/// <param name="AssignmentId">The assignment that ended.</param>
/// <param name="ApartmentId">The apartment that became free.</param>
/// <param name="ResidentId">The resident who moved out.</param>
public sealed record ResidentMovedOut(
    Guid AssignmentId,
    Guid ApartmentId,
    Guid ResidentId) : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
