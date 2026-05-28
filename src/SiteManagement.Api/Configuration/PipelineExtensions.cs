using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization;
using Serilog;
using SiteManagement.Api.Messaging;
using SiteManagement.Api.Middleware;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Composes the request pipeline in the correct order: request localization
/// runs first so the global exception middleware (and every handler) sees
/// the right culture; then global exception → request logging → dev-only
/// OpenAPI + Scalar → auth → endpoints.
/// </summary>
public static class PipelineExtensions
{
    /// <summary>Installs the full SiteManagement HTTP pipeline.</summary>
    public static WebApplication UseSiteManagementPipeline(this WebApplication app)
    {
        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);

        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseSerilogRequestLogging();

        // CORS picks the wide-open dev policy in Development and the
        // config-driven whitelist in every other environment. Must come
        // before auth so preflight (OPTIONS) responses don't 401.
        app.UseCors(app.Environment.IsDevelopment()
            ? CorsExtensions.DevelopmentPolicy
            : CorsExtensions.ProductionPolicy);

        if (app.Environment.IsDevelopment())
        {
            app.MapSiteManagementOpenApi();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks(ApiConstants.HealthEndpoint);
        app.MapGet(ApiConstants.HealthLiveEndpoint, () => Results.Ok(new { status = "live" }));

        app.MapControllers();
        app.MapMessagingHub();

        return app;
    }
}
