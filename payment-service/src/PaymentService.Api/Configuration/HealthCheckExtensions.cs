using Microsoft.Extensions.Diagnostics.HealthChecks;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Api.Configuration;

/// <summary>
/// Registers a Mongo-backed liveness probe so <c>/health</c> reports the
/// service healthy only when the database answers a ping.
/// </summary>
public static class HealthCheckExtensions
{
    private const string MongoCheckName = "mongo";

    /// <summary>Adds the payment-service health checks.</summary>
    public static IServiceCollection AddPaymentHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck<MongoHealthCheck>(MongoCheckName);
        return services;
    }

    /// <summary>Pings Mongo through the shared context to confirm connectivity.</summary>
    private sealed class MongoHealthCheck(PaymentMongoContext context) : IHealthCheck
    {
        private readonly PaymentMongoContext _context = context;

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.PingAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("MongoDB ping failed.", ex);
            }
        }
    }
}
