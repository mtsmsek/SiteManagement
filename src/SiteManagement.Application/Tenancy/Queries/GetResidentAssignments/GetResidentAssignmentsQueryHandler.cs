using MediatR;

namespace SiteManagement.Application.Tenancy.Queries.GetResidentAssignments;

/// <summary>Delegates straight to <see cref="ITenancyQueries"/> — pure read path.</summary>
public sealed class GetResidentAssignmentsQueryHandler(ITenancyQueries tenancyQueries)
    : IRequestHandler<GetResidentAssignmentsQuery, IReadOnlyList<ResidentAssignmentDto>>
{
    private readonly ITenancyQueries _tenancyQueries = tenancyQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<ResidentAssignmentDto>> Handle(GetResidentAssignmentsQuery request, CancellationToken cancellationToken)
        => _tenancyQueries.GetAssignmentsForResidentAsync(request.ResidentId, cancellationToken);
}
