using MediatR;

namespace SiteManagement.Application.Reports.Queries.GetAdminDashboard;

/// <summary>Delegates straight to <see cref="IReportQueries"/> — pure read path.</summary>
public sealed class GetAdminDashboardQueryHandler(IReportQueries reportQueries)
    : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IReportQueries _reportQueries = reportQueries;

    /// <inheritdoc />
    public Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        => _reportQueries.GetAdminDashboardAsync(cancellationToken);
}
