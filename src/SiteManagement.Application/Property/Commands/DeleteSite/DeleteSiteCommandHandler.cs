using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.DeleteSite;

/// <summary>
/// Soft-deletes (archives) the site: the row stays put with its IsDeleted flag
/// set, and a global query filter hides it from every read so blocks, apartments
/// and dependent history survive. <c>PurgeSiteCommand</c> is the explicit,
/// admin-only hard delete.
/// </summary>
public sealed class DeleteSiteCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
    : IRequestHandler<DeleteSiteCommand>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly TimeProvider _clock = clock;

    /// <inheritdoc />
    public async Task Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Site), request.SiteId);

        site.Archive(_clock.GetUtcNow().UtcDateTime);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
