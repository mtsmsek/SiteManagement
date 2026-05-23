using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.RestoreSite;

/// <summary>
/// Restores an archived site. Loads it bypassing the soft-delete filter (it is
/// hidden by definition) and clears the IsDeleted flag. TransactionBehavior
/// wraps the save.
/// </summary>
public sealed class RestoreSiteCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RestoreSiteCommand>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(RestoreSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.FindIncludingArchivedAsync(request.SiteId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Site), request.SiteId);

        site.Restore();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
