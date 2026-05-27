using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Reports.Queries;
using SiteManagement.Application.Reports.Queries.GetAdminDashboard;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Api.Controllers.Reports;

/// <summary>
/// Admin reporting endpoints. Cross-cutting read-side projections that span more
/// than one bounded context (sites + residents + billing).
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route($"{ApiConstants.RoutePrefix}/reports")]
public class ReportsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>System-wide dashboard totals (counts, accrued/collected, outstanding, credit, collection rate).</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType<AdminDashboardDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminDashboardDto>> Dashboard(CancellationToken ct)
        => Ok(await _sender.Send(new GetAdminDashboardQuery(), ct));
}
