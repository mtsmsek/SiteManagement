using SiteManagement.Api.Configuration;
using SiteManagement.Api.Messaging;
using SiteManagement.Application;
using SiteManagement.Infrastructure;
using SiteManagement.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// Platform-injected env vars (Railway / Render / Heroku style): bind to $PORT
// and translate DATABASE_URL to Npgsql syntax. No-ops locally.
builder.UsePlatformPort();
builder.UsePlatformDatabaseUrl();

builder.Host.AddSerilogLogging();

builder.Services
    .AddControllers()
    .Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddCurrentUser()
    .AddMessagingHub()
    .AddSiteManagementRateLimiter()
    .AddSiteManagementCors(builder.Configuration)
    .AddSiteManagementHealthChecks(builder.Configuration)
    .AddSiteManagementLocalization()
    .AddSiteManagementOpenApi();

var app = builder.Build();

app.UseSiteManagementPipeline();

await DatabaseInitializer.MigrateAndSeedAsync(app.Services);

app.Run();

/// <summary>Exposes <c>Program</c> as a partial class so WebApplicationFactory can target it from E2E tests.</summary>
public partial class Program;
