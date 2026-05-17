using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Wires the request-culture middleware: detect the caller's language from
/// the <c>Accept-Language</c> header (or, in W2, a query-string override),
/// and set <see cref="System.Globalization.CultureInfo.CurrentCulture"/> +
/// <see cref="CultureInfo.CurrentUICulture"/> so the
/// <c>IStringLocalizer&lt;T&gt;</c> instances injected into the middleware
/// resolve resources for that culture.
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>Default culture when the caller doesn't send <c>Accept-Language</c>.</summary>
    public const string DefaultCulture = "tr-TR";

    /// <summary>Fallback English culture for international clients.</summary>
    public const string EnglishCulture = "en-US";

    /// <summary>Supported cultures advertised to the localization pipeline.</summary>
    public static readonly string[] SupportedCultures = [DefaultCulture, EnglishCulture];

    /// <summary>Registers request-localization options based on the configured culture list.</summary>
    public static IServiceCollection AddSiteManagementLocalization(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(opts =>
        {
            var cultures = SupportedCultures.Select(c => new CultureInfo(c)).ToArray();

            opts.DefaultRequestCulture = new RequestCulture(DefaultCulture);
            opts.SupportedCultures = cultures;
            opts.SupportedUICultures = cultures;

            // ASP.NET Core's default providers (query string `culture=tr-TR`,
            // cookie, Accept-Language header) are kept in their default order:
            // the header is the realistic source for a stateless API.
        });

        return services;
    }
}
