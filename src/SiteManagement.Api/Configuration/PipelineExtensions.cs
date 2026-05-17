using Serilog;
using SiteManagement.Api.Middleware;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Composes the request pipeline in the correct order: global exception
/// handler → request logging → dev-only OpenAPI + Scalar → auth → endpoints.
/// </summary>
public static class PipelineExtensions
{
    /// <summary>Installs the full SiteManagement HTTP pipeline.</summary>
    public static WebApplication UseSiteManagementPipeline(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapSiteManagementOpenApi();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks(ApiConstants.HealthEndpoint);
        app.MapGet(ApiConstants.HealthLiveEndpoint, () => Results.Ok(new { status = "live" }));

        app.MapControllers();

        return app;
    }
}
