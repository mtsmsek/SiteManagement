using SiteManagement.Domain.Shared;

namespace SiteManagement.Application.Abstractions.Events;

/// <summary>
/// Publishes domain events raised by aggregates to their in-process handlers.
/// The Application layer depends on this port; the Infrastructure layer wires
/// it to MediatR. Keeping it behind a port lets the unit of work flush events
/// without taking a direct MediatR dependency, and lets tests assert dispatch
/// without a container.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>Dispatches each event to every registered handler, in order.</summary>
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default);
}
