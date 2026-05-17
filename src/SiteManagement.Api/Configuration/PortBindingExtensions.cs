namespace SiteManagement.Api.Configuration;

/// <summary>
/// Platforms like Railway and Render inject a dynamic <c>PORT</c> environment
/// variable and expect the app to bind to it. Kestrel doesn't read that
/// variable by default, so this helper bridges it to
/// <c>ASPNETCORE_URLS</c> when the platform variable is present.
/// Local <c>docker compose</c> runs are unaffected: when <c>PORT</c> is not
/// set, this method is a no-op and the existing
/// <c>ASPNETCORE_HTTP_PORTS</c>/<c>ASPNETCORE_URLS</c> values stay in charge.
/// </summary>
public static class PortBindingExtensions
{
    private const string PlatformPortVariable = "PORT";
    private const string AspNetCoreUrlsVariable = "ASPNETCORE_URLS";

    /// <summary>
    /// If a host-injected <c>PORT</c> is present and <c>ASPNETCORE_URLS</c>
    /// is empty, sets <c>ASPNETCORE_URLS</c> to <c>http://+:$PORT</c>.
    /// </summary>
    public static WebApplicationBuilder UsePlatformPort(this WebApplicationBuilder builder)
    {
        var port = Environment.GetEnvironmentVariable(PlatformPortVariable);
        var explicitUrls = Environment.GetEnvironmentVariable(AspNetCoreUrlsVariable);

        if (!string.IsNullOrWhiteSpace(port) && string.IsNullOrWhiteSpace(explicitUrls))
        {
            builder.WebHost.UseUrls($"http://+:{port}");
        }

        return builder;
    }
}
