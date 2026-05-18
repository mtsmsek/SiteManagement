using MediatR;

namespace SiteManagement.Application.Residency.Queries.ListResidents;

/// <summary>Pure pass-through to <see cref="IResidentQueries"/>.</summary>
public sealed class ListResidentsQueryHandler(IResidentQueries residentQueries)
    : IRequestHandler<ListResidentsQuery, IReadOnlyList<ResidentListItemDto>>
{
    private readonly IResidentQueries _residentQueries = residentQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<ResidentListItemDto>> Handle(
        ListResidentsQuery request, CancellationToken cancellationToken)
        => _residentQueries.ListAsync(cancellationToken);
}
