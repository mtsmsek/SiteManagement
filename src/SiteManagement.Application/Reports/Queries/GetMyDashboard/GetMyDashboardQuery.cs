using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Application.Reports.Queries;

namespace SiteManagement.Application.Reports.Queries.GetMyDashboard;

/// <summary>The current resident's portal summary (own outstanding, credit, unread messages).</summary>
public sealed record GetMyDashboardQuery : IQuery<ResidentDashboardDto>, IResidentRequest;
