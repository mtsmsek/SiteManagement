using FluentAssertions;
using MediatR;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Domain.Shared;

namespace SiteManagement.ArchitectureTests;

/// <summary>
/// Guards the integration-event convention so the outbox split can't bite a
/// future contributor. An integration event is routed to the outbox by the
/// unit of work and delivered after commit; if nobody handles it, the side
/// effect (email, …) silently never happens — exactly the "why didn't the mail
/// go out?" debugging session this test exists to prevent.
/// </summary>
public class IntegrationEventConventionsTests
{
    /// <summary>Every <see cref="IIntegrationEvent"/> must have at least one handler.</summary>
    [Fact]
    public void Every_IntegrationEvent_HasAtLeastOneHandler()
    {
        // arrange — concrete integration events declared in the Domain
        var integrationEvents = AssemblyReferences.Domain
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IIntegrationEvent).IsAssignableFrom(t))
            .ToArray();

        // the event types wrapped by a DomainEventNotification<T> handler in the Application
        var handledEventTypes = AssemblyReferences.Application
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces())
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
            .Select(i => i.GetGenericArguments()[0])
            .Where(n => n.IsGenericType && n.GetGenericTypeDefinition() == typeof(DomainEventNotification<>))
            .Select(n => n.GetGenericArguments()[0])
            .ToHashSet();

        // assert
        var orphans = integrationEvents
            .Where(e => !handledEventTypes.Contains(e))
            .Select(e => e.FullName!)
            .ToArray();

        orphans.Should().BeEmpty(
            "every integration event must have a handler or it silently never delivers (orphans: {0})",
            string.Join(", ", orphans));
    }
}
