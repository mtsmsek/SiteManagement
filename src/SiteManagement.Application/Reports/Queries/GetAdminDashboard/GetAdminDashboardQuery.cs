using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Application.Reports.Queries;

namespace SiteManagement.Application.Reports.Queries.GetAdminDashboard;

/// <summary>System-wide admin dashboard totals.</summary>
public sealed record GetAdminDashboardQuery : IQuery<AdminDashboardDto>, IAdminRequest;
