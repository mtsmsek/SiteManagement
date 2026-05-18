using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Application.Property.Commands.AddApartment;

/// <summary>
/// Hydrates the <see cref="Site"/> aggregate that owns the targeted block,
/// then asks the block to add an apartment. <see cref="Block.AddApartment"/>
/// enforces the per-block number-uniqueness invariant; not-found branches
/// surface as <see cref="EntityNotFoundException"/> 404s before the domain
/// is reached.
/// </summary>
public sealed class AddApartmentCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddApartmentCommand, AddApartmentResult>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<AddApartmentResult> Handle(AddApartmentCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.FindContainingBlockAsync(request.BlockId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Block), request.BlockId);

        var block = site.GetBlock(request.BlockId);

        var apartment = Apartment.Create(
            ApartmentNumber.From(request.Number),
            Floor.From(request.Floor),
            ApartmentType.From(request.Type));

        block.AddApartment(apartment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddApartmentResult(apartment.Id);
    }
}
