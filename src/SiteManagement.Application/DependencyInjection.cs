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

            // Order matters (outermost first): logging wraps the whole call,
            // authorization rejects a disallowed caller before any work (so an
            // unauthorized request is never validated or executed), validation
            // runs before the handler, exception translation catches anything
            // the handler (or domain) throws, then the transaction sits closest
            // to the handler so a thrown-and-translated error still leaves the
            // scope uncommitted (rolled back on dispose).
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(ExceptionTranslationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: false);

        // Billing orchestration that spans more than one aggregate but isn't a
        // request handler itself (shared by the dues + utility amount-change and
        // distribute handlers).
        services.AddScoped<Billing.Services.IResidentCreditService, Billing.Services.ResidentCreditService>();

        return services;
    }
}
