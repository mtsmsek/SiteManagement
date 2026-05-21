using MediatR;

namespace SiteManagement.Application.Tenancy.Queries.GetSiteOccupants;

/// <summary>Delegates straight to <see cref="ITenancyQueries"/> — pure read path.</summary>
public sealed class GetSiteOccupantsQueryHandler(ITenancyQueries tenancyQueries)
    : IRequestHandler<GetSiteOccupantsQuery, IReadOnlyList<ApartmentOccupantDto>>
{
    private readonly ITenancyQueries _tenancyQueries = tenancyQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<ApartmentOccupantDto>> Handle(GetSiteOccupantsQuery request, CancellationToken cancellationToken)
        => _tenancyQueries.GetActiveOccupantsForSiteAsync(request.SiteId, cancellationToken);
}
