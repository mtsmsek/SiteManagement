using SiteManagement.Api.Configuration;
using SiteManagement.Application;
using SiteManagement.Infrastructure;
using SiteManagement.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging();

builder.Services
    .AddControllers()
    .Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddSiteManagementHealthChecks(builder.Configuration)
    .AddSiteManagementOpenApi();

var app = builder.Build();

app.UseSiteManagementPipeline();

await DatabaseInitializer.MigrateAndSeedAsync(app.Services);

app.Run();

/// <summary>Exposes <c>Program</c> as a partial class so WebApplicationFactory can target it from E2E tests.</summary>
public partial class Program;
