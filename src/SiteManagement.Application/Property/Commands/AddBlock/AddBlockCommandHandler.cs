using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Application.Property.Commands.AddBlock;

/// <summary>
/// Loads the targeted <see cref="Site"/> aggregate and delegates the
/// add-block invariant (case-insensitive name uniqueness) to it. The
/// domain raises <see cref="Domain.Property.Exceptions.DuplicateBlockNameException"/>
/// when the name already exists; the MediatR pipeline turns it into a
/// localized <c>BusinessRuleViolationException</c>.
/// </summary>
public sealed class AddBlockCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddBlockCommand, AddBlockResult>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<AddBlockResult> Handle(AddBlockCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Site), request.SiteId);

        var block = site.AddBlock(BlockName.From(request.Name));

        // EF treats a brand-new child added through a domain method on a
        // tracked aggregate as "modified" by default, which makes the
        // change tracker emit an UPDATE against a row that doesn't exist
        // yet. Flip the entry state to "added" so the INSERT goes out.
        _unitOfWork.MarkAsAdded(block);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddBlockResult(block.Id);
    }
}
