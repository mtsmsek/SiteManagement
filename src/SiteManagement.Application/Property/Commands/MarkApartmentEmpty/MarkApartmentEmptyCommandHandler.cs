using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.MarkApartmentEmpty;

/// <summary>
/// Hydrates the owning <see cref="Site"/> aggregate and asks the
/// apartment to transition back to <see cref="OccupancyStatus.Empty"/>.
/// Domain rejects when already empty.
/// </summary>
public sealed class MarkApartmentEmptyCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MarkApartmentEmptyCommand>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(MarkApartmentEmptyCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.FindContainingApartmentAsync(request.ApartmentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Apartment), request.ApartmentId);

        var apartment = site.Blocks.SelectMany(b => b.Apartments)
            .First(a => a.Id == request.ApartmentId);

        apartment.MarkAsEmpty();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
