using MediatR;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Property.Queries.GetSiteById;

/// <summary>
/// Delegates to <see cref="ISiteQueries"/>; surfaces the 404 path as
/// <see cref="EntityNotFoundException"/> so the API middleware renders
/// the canonical ProblemDetails response.
/// </summary>
public sealed class GetSiteByIdQueryHandler(ISiteQueries siteQueries)
    : IRequestHandler<GetSiteByIdQuery, SiteDetailsDto>
{
    private readonly ISiteQueries _siteQueries = siteQueries;

    /// <inheritdoc />
    public async Task<SiteDetailsDto> Handle(GetSiteByIdQuery request, CancellationToken cancellationToken)
        => await _siteQueries.GetByIdAsync(request.SiteId, cancellationToken)
           ?? throw new EntityNotFoundException(nameof(Site), request.SiteId);
}
