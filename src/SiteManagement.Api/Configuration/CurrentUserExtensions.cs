using SiteManagement.Application.Abstractions.Auth;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// DI wiring for the HttpContext-backed <see cref="ICurrentUser"/>.
/// Lives in the Api layer because <see cref="IHttpContextAccessor"/> is an
/// ASP.NET Core concept; the Application layer only sees the
/// <see cref="ICurrentUser"/> port.
/// </summary>
public static class CurrentUserExtensions
{
    /// <summary>Registers <see cref="IHttpContextAccessor"/> + the scoped <see cref="ICurrentUser"/>.</summary>
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        return services;
    }
}
