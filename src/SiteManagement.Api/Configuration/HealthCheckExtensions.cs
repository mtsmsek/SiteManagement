using SiteManagement.Infrastructure;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Registers the application's readiness probes. The Postgres connection is
/// the single external dependency at this stage; W4 adds Mongo for the
/// payment service.
/// </summary>
public static class HealthCheckExtensions
{
    private const string PostgresProbeName = "postgres";

    /// <summary>Adds a Postgres probe under the readiness health-check pipeline.</summary>
    public static IServiceCollection AddSiteManagementHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(
                connectionStringFactory: _ => configuration.GetConnectionString(
                    DependencyInjection.ConnectionStringName)!,
                name: PostgresProbeName);

        return services;
    }
}
