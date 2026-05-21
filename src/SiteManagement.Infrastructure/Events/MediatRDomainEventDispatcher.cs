using MediatR;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Infrastructure.Events;

/// <summary>
/// <see cref="IDomainEventDispatcher"/> backed by MediatR. Each domain event is
/// wrapped in a <see cref="DomainEventNotification{TDomainEvent}"/> (built via
/// reflection because the concrete event type is only known at runtime) and
/// published, so the Domain layer never references MediatR.
/// </summary>
public sealed class MediatRDomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    private readonly IPublisher _publisher = publisher;

    /// <inheritdoc />
    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent)!;
            await _publisher.Publish(notification, ct);
        }
    }
}
