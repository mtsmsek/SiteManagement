using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiteManagement.Api.HealthChecks;

/// <summary>
/// Probes the downstream Payment service's own <c>/health</c> endpoint
/// through a typed <see cref="HttpClient"/>. Failures are surfaced as
/// <see cref="HealthStatus.Unhealthy"/> so the orchestrator (and the
/// main API's own healthcheck) sees the dependency outage directly,
/// rather than waiting for a charge to fail.
/// </summary>
public sealed class PaymentServiceHealthCheck(HttpClient httpClient) : IHealthCheck
{
    /// <summary>Named <see cref="HttpClient"/> used by this probe; registered in HealthCheckExtensions.</summary>
    public const string HttpClientName = "payment-service-health";

    private readonly HttpClient _httpClient = httpClient;

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy(
                    $"PaymentService responded with HTTP {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PaymentService is unreachable.", ex);
        }
    }
}
