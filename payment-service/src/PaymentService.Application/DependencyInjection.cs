using Microsoft.Extensions.DependencyInjection;

namespace PaymentService.Application;

/// <summary>
/// Composition root for the Application layer. Use-case handlers + validators
/// are registered here as they arrive (day 3). Exists now so the Api's
/// Program.cs can chain it and stay lean.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers Application-layer services on the supplied collection.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Handlers + validators wired in day 3.
        return services;
    }
}
