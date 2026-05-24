using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer: binds Mongo settings and
/// registers the Mongo context. Repositories are added here as they arrive
/// (day 3). Keeps the Api's Program.cs lean.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers MongoDB persistence on the supplied collection.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MongoOptions>()
            .Bind(configuration.GetSection(MongoOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<PaymentMongoContext>();

        return services;
    }
}
