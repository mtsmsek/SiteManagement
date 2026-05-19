using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Wires JWT bearer authentication and authorization. Lives next to
/// <see cref="JwtOptions"/> so the auth scheme details stay with the
/// infrastructure that owns them.
/// </summary>
public static class AuthExtensions
{
    /// <summary>
    /// Registers the JWT bearer scheme. The
    /// <see cref="TokenValidationParameters"/> are built lazily from
    /// <see cref="IOptions{JwtOptions}"/> at request-resolve time so any
    /// in-memory configuration override applied by tests (or
    /// <c>WebApplicationFactory</c>) is honoured.
    /// </summary>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        // Bind JwtBearerOptions lazily — reads the same IOptions<JwtOptions>
        // that TokenService uses, so the signing key + issuer + audience
        // stay consistent even when ConfigureAppConfiguration overrides
        // them after AddJwtAuth registration (e.g. in integration tests).
        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
            {
                var options = jwt.Value;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        services.AddAuthorization();
        return services;
    }
}
