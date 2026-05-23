using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.PurgeSite;

/// <summary>
/// Hard-deletes a site for good. Loads it bypassing the soft-delete filter (so
/// an archived site can still be purged) and lets EF Core's cascade remove the
/// blocks + apartments physically. TransactionBehavior wraps the save.
/// </summary>
public sealed class PurgeSiteCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PurgeSiteCommand>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(PurgeSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.FindIncludingArchivedAsync(request.SiteId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Site), request.SiteId);

        _siteRepository.Remove(site);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
