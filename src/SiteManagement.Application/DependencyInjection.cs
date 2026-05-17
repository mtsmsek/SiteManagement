using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using SiteManagement.Application.Behaviors;

namespace SiteManagement.Application;

/// <summary>
/// Composition root for the Application layer. Registers MediatR (with the
/// three cross-cutting pipeline behaviors in the correct order) and every
/// FluentValidation validator discovered in this assembly.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Application-layer services on the supplied collection and
    /// returns it for chaining.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).GetTypeInfo().Assembly;

        // Localization stays inside Application because resources live here.
        // W1 Day 6 adds the .resx files; until then the localizer returns the
        // raw key, which is exactly what we want as a fallback.
        services.AddLocalization();
        services.TryAddSingleton(typeof(IStringLocalizerFactory), typeof(ResourceManagerStringLocalizerFactory));
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Order matters: logging wraps the whole call, validation runs
            // before the handler, exception translation catches anything the
            // handler (or domain) throws.
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(ExceptionTranslationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: false);

        return services;
    }
}
