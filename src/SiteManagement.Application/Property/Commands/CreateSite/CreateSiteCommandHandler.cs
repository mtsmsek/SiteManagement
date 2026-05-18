using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Commands.CreateSite;

/// <summary>
/// Creates a brand-new <see cref="Site"/> aggregate. Single-write command,
/// so it uses the implicit unit of work (one <c>SaveChangesAsync</c> at
/// the end) instead of opening an explicit transaction scope.
/// </summary>
public sealed class CreateSiteCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSiteCommand, CreateSiteResult>
{
    private readonly ISiteRepository _siteRepository = siteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<CreateSiteResult> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
    {
        var site = Site.Create(request.Name, request.Address);
        await _siteRepository.AddAsync(site, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateSiteResult(site.Id);
    }
}
