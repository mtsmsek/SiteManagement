using MediatR;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Infrastructure.Events;

/// <summary>
/// MediatR adapter that wraps a domain event so it can travel through the
/// notification pipeline without the Domain layer ever referencing MediatR.
/// Application-layer handlers implement
/// <c>INotificationHandler&lt;DomainEventNotification&lt;TEvent&gt;&gt;</c>.
/// </summary>
/// <typeparam name="TDomainEvent">The wrapped domain event type.</typeparam>
/// <param name="DomainEvent">The event raised by an aggregate.</param>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
