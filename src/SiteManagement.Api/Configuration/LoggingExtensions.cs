using Serilog;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Serilog wiring used at startup. Reads sinks + minimum level from
/// configuration (<c>Serilog</c> section in appsettings) so deployments can
/// re-configure logging without rebuilding the image.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Plugs Serilog into the host as the standard <c>ILogger</c> provider.
    /// </summary>
    public static IHostBuilder AddSerilogLogging(this IHostBuilder host) => host.UseSerilog((ctx, sp, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(sp)
        .Enrich.FromLogContext());
}
