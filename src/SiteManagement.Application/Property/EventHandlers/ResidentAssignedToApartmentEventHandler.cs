using MediatR;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Tenancy.Events;

namespace SiteManagement.Application.Property.EventHandlers;

/// <summary>
/// Marks an apartment occupied when a resident is assigned to it. Runs inside
/// the same unit-of-work flow that raised the event (see EfUnitOfWork's
/// dispatch loop), so the occupancy change is saved in the same transaction as
/// the assignment. The apartment's state machine ignores a redundant
/// transition, so the handler stays simple.
/// </summary>
public sealed class ResidentAssignedToApartmentEventHandler(ISiteRepository siteRepository)
    : INotificationHandler<DomainEventNotification<ResidentAssignedToApartment>>
{
    private readonly ISiteRepository _siteRepository = siteRepository;

    /// <inheritdoc />
    public async Task Handle(
        DomainEventNotification<ResidentAssignedToApartment> notification,
        CancellationToken cancellationToken)
    {
        var apartmentId = notification.DomainEvent.ApartmentId;
        var site = await _siteRepository.FindContainingApartmentAsync(apartmentId, cancellationToken);
        if (site is null)
        {
            return;
        }

        var apartment = site.Blocks.SelectMany(b => b.Apartments).First(a => a.Id == apartmentId);
        apartment.MarkAsOccupied();
    }
}
