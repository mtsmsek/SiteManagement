using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// CORS wiring. Two policies are registered side by side:
/// <list type="bullet">
///   <item><description><see cref="DevelopmentPolicy"/> — applied automatically
///         when the host is in Development; allows the Angular dev server
///         (<c>http://localhost:4200</c>) plus a configurable list of extra
///         origins (e.g. another teammate's machine).</description></item>
///   <item><description><see cref="ProductionPolicy"/> — applied in non-dev
///         environments; the allowed origins come strictly from the
///         <c>Cors:AllowedOrigins</c> configuration array.</description></item>
/// </list>
/// </summary>
public static class CorsExtensions
{
    /// <summary>CORS policy name used during local development.</summary>
    public const string DevelopmentPolicy = "SiteManagement.Dev";

    /// <summary>CORS policy name used in non-development environments.</summary>
    public const string ProductionPolicy = "SiteManagement.Prod";

    /// <summary>Configuration section consumed by both policies.</summary>
    public const string SectionName = "Cors";

    /// <summary>Default Angular dev URL — always allowed in Development.</summary>
    public const string DefaultDevOrigin = "http://localhost:4200";

    /// <summary>
    /// Registers both CORS policies. The pipeline (<c>UseCorsPolicy</c>)
    /// picks the right one at runtime based on the hosting environment.
    /// </summary>
    public static IServiceCollection AddSiteManagementCors(this IServiceCollection services, IConfiguration configuration)
    {
        var configuredOrigins = configuration.GetSection(SectionName).GetSection("AllowedOrigins").Get<string[]>()
            ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(DevelopmentPolicy, policy => policy
                .WithOrigins([DefaultDevOrigin, .. configuredOrigins])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            options.AddPolicy(ProductionPolicy, policy => policy
                .WithOrigins(configuredOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        return services;
    }
}
