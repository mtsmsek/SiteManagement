using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SiteManagement.Api.HealthChecks;
using SiteManagement.Infrastructure;
using SiteManagement.Infrastructure.Payments;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Registers the application's readiness probes: the Postgres connection and
/// the downstream Payment service (an HTTP probe through a typed client).
/// Mongo is the Payment service's own dependency — that service exposes its
/// own <c>/health</c>, which this API's probe transitively reflects.
/// </summary>
public static class HealthCheckExtensions
{
    private const string PostgresProbeName = "postgres";
    private const string PaymentServiceProbeName = "payment-service";

    /// <summary>Adds the Postgres + Payment-service probes under the readiness pipeline.</summary>
    public static IServiceCollection AddSiteManagementHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(
                connectionStringFactory: _ => configuration.GetConnectionString(
                    DependencyInjection.ConnectionStringName)!,
                name: PostgresProbeName)
            .AddCheck<PaymentServiceHealthCheck>(
                PaymentServiceProbeName,
                failureStatus: HealthStatus.Unhealthy);

        // Named HttpClient consumed by PaymentServiceHealthCheck. Tight 2-second
        // ceiling so a hung downstream cannot stall the readiness probe.
        services.AddHttpClient<PaymentServiceHealthCheck>(PaymentServiceHealthCheck.HttpClientName, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<PaymentServiceOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }
            client.Timeout = TimeSpan.FromSeconds(2);
        });

        return services;
    }
}
