using MediatR;

namespace SiteManagement.Application.Property.Queries.ListSites;

/// <summary>Delegates straight to <see cref="ISiteQueries"/> — pure read path.</summary>
public sealed class ListSitesQueryHandler(ISiteQueries siteQueries)
    : IRequestHandler<ListSitesQuery, IReadOnlyList<SiteListItemDto>>
{
    private readonly ISiteQueries _siteQueries = siteQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<SiteListItemDto>> Handle(ListSitesQuery request, CancellationToken cancellationToken)
        => _siteQueries.ListAsync(cancellationToken);
}
