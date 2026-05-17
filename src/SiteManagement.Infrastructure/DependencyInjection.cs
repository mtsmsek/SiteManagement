using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Infrastructure.Auth;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;

namespace SiteManagement.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. Wires up EF Core +
/// PostgreSQL, ASP.NET Core Identity, and concrete implementations of the
/// Application-layer auth ports.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Name of the connection string read from configuration.</summary>
    public const string ConnectionStringName = "DefaultConnection";

    /// <summary>
    /// Registers EF Core, Identity, and the auth services on the supplied
    /// collection and returns it for chaining.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is missing from configuration.");

        services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(
            connectionString,
            npg => npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

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

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserAuthService, UserAuthService>();
        services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();

        return services;
    }
}
