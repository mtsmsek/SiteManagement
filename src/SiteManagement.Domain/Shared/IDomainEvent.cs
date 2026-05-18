namespace SiteManagement.Domain.Shared;

/// <summary>
/// Marker for "something meaningful happened inside an aggregate". Concrete
/// events are immutable records (e.g. <c>ApartmentOccupied</c>). The
/// Infrastructure layer dispatches them to MediatR handlers after a
/// successful save.
/// </summary>
public interface IDomainEvent
{
    /// <summary>The moment the event was raised, in UTC.</summary>
    DateTime OccurredOnUtc { get; }
}
