using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Email;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Infrastructure.Auth;
using SiteManagement.Infrastructure.Email;
using SiteManagement.Infrastructure.Events;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Payments;
using SiteManagement.Infrastructure.Persistence;

namespace SiteManagement.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. Delegates to focused
/// per-concern extensions (persistence, identity, auth) so each wireup
/// lives where the rest of its code lives instead of bloating one method.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Re-exported so the Api layer can read it without taking a direct PersistenceExtensions dependency.</summary>
    public const string ConnectionStringName = PersistenceExtensions.ConnectionStringName;

    /// <summary>Registers every Infrastructure service in the right scope.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence();

        services
            .AddIdentity<AppUser, AppRole>(opts =>
            {
                opts.Password.RequiredLength = IdentityPasswordPolicy.RequiredLength;
                opts.Password.RequireDigit = IdentityPasswordPolicy.RequireDigit;
                opts.Password.RequireLowercase = IdentityPasswordPolicy.RequireLowercase;
                opts.Password.RequireUppercase = IdentityPasswordPolicy.RequireUppercase;
                opts.Password.RequireNonAlphanumeric = IdentityPasswordPolicy.RequireNonAlphanumeric;
                opts.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AdminBootstrapOptions>()
            .Bind(configuration.GetSection(AdminBootstrapOptions.SectionName));

        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName));

        services.AddOptions<OutboxOptions>()
            .Bind(configuration.GetSection(OutboxOptions.SectionName));

        services.AddOptions<Persistence.Seed.DemoOptions>()
            .Bind(configuration.GetSection(Persistence.Seed.DemoOptions.SectionName));

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserAuthService, UserAuthService>();
        services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
        services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        // Outbox: after-commit delivery of integration events.
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddHostedService<OutboxBackgroundService>();

        // Outbound Payment-service integration (Refit + resilience + adapter).
        services.AddPaymentGateway(configuration);

        return services;
    }
}
