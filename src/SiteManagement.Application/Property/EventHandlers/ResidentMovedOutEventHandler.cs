using MediatR;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Tenancy.Events;

namespace SiteManagement.Application.Property.EventHandlers;

/// <summary>
/// Frees an apartment (marks it empty) when its assignment ends — the
/// move-out counterpart of <see cref="ResidentAssignedToApartmentEventHandler"/>.
/// Runs in the same unit-of-work flow that raised the event, so the occupancy
/// change commits with the move-out.
/// </summary>
public sealed class ResidentMovedOutEventHandler(ISiteRepository siteRepository)
    : INotificationHandler<DomainEventNotification<ResidentMovedOut>>
{
    private readonly ISiteRepository _siteRepository = siteRepository;

    /// <inheritdoc />
    public async Task Handle(
        DomainEventNotification<ResidentMovedOut> notification,
        CancellationToken cancellationToken)
    {
        var apartmentId = notification.DomainEvent.ApartmentId;
        var site = await _siteRepository.FindContainingApartmentAsync(apartmentId, cancellationToken);
        if (site is null)
        {
            return;
        }

        var apartment = site.Blocks.SelectMany(b => b.Apartments).First(a => a.Id == apartmentId);
        apartment.MarkAsEmpty();
    }
}
