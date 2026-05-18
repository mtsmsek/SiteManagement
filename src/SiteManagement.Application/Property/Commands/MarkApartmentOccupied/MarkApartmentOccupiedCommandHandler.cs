using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.MarkApartmentOccupied;

/// <summary>
/// Hydrates the owning <see cref="Site"/> aggregate and delegates the
/// occupancy transition to the <see cref="Apartment"/> entity, whose state
/// machine rejects double-occupy with a domain exception.
/// </summary>
public sealed class MarkApartmentOccupiedCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MarkApartmentOccupiedCommand>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(MarkApartmentOccupiedCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.FindContainingApartmentAsync(request.ApartmentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Apartment), request.ApartmentId);

        var apartment = site.Blocks.SelectMany(b => b.Apartments)
            .First(a => a.Id == request.ApartmentId);

        apartment.MarkAsOccupied();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
