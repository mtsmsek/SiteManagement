namespace SiteManagement.Application.Reports.Queries;

/// <summary>
/// Read-side port for cross-cutting admin reports that span more than one
/// bounded context (sites + residents + billing). Returns DTOs only.
/// </summary>
public interface IReportQueries
{
    /// <summary>System-wide dashboard totals: counts, accrued/collected, outstanding, credit, collection rate.</summary>
    Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken ct = default);
}
