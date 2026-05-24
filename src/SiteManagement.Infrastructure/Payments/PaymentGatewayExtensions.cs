using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SiteManagement.Application.Abstractions.Payments;

namespace SiteManagement.Infrastructure.Payments;

/// <summary>
/// Wires the outbound Payment-service integration: the Refit HTTP client (with
/// base URL + API-key header), a standard resilience pipeline (retry, timeout,
/// circuit breaker) on that client, and the adapter that exposes it through the
/// Application's <see cref="IPaymentGateway"/> port.
/// </summary>
public static class PaymentGatewayExtensions
{
    private const string ApiKeyHeader = "X-Api-Key";

    /// <summary>Registers the Payment-service client, resilience, and gateway adapter.</summary>
    public static IServiceCollection AddPaymentGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PaymentServiceOptions>()
            .Bind(configuration.GetSection(PaymentServiceOptions.SectionName))
            .ValidateOnStart();

        services
            .AddRefitClient<IPaymentServiceApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PaymentServiceOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                if (!string.IsNullOrEmpty(options.ApiKey))
                {
                    client.DefaultRequestHeaders.Add(ApiKeyHeader, options.ApiKey);
                }
            })
            // Retry transient faults, time out a hung call, and trip a circuit
            // breaker if the Payment service keeps failing — so a slow/down
            // dependency degrades cleanly instead of hanging the request.
            .AddStandardResilienceHandler();

        services.AddScoped<IPaymentGateway, PaymentGatewayAdapter>();

        return services;
    }
}
