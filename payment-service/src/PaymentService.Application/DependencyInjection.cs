using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Payments;

namespace PaymentService.Application;

/// <summary>
/// Composition root for the Application layer. Registers the payment use case
/// and the system clock it depends on. Keeps the Api's Program.cs lean.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers Application-layer services on the supplied collection.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IPaymentProcessor, PaymentProcessor>();
        return services;
    }
}
