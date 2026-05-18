using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.DeleteSite;

/// <summary>
/// Removes the site aggregate. EF Core's cascade delete (configured in
/// SiteConfiguration) takes care of blocks + apartments.
/// </summary>
public sealed class DeleteSiteCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSiteCommand>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Site), request.SiteId);

        _siteRepository.Remove(site);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
