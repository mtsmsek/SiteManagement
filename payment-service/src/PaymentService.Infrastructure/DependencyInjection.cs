using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Abstractions;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Persistence.Repositories;

namespace PaymentService.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer: binds Mongo settings,
/// registers the Mongo context and the aggregate repositories. Keeps the
/// Api's Program.cs lean.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers MongoDB persistence + repositories on the supplied collection.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MongoOptions>()
            .Bind(configuration.GetSection(MongoOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<PaymentMongoContext>();

        services.AddScoped<IBankAccountRepository, MongoBankAccountRepository>();
        services.AddScoped<ICreditCardRepository, MongoCreditCardRepository>();
        services.AddScoped<IPaymentTransactionRepository, MongoPaymentTransactionRepository>();

        return services;
    }
}
