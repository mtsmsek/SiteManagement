using System.Net;
using SiteManagement.Infrastructure;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Translates the platform-style <c>DATABASE_URL</c> connection string
/// (<c>postgresql://user:pass@host:port/db</c>) that Railway, Render, Heroku
/// and similar PaaS providers inject into the keyword/value form Npgsql
/// expects, and overwrites
/// <c>ConnectionStrings:DefaultConnection</c> with the result.
/// </summary>
/// <remarks>
/// No-op when <c>DATABASE_URL</c> is missing, so local docker-compose
/// runs (which set <c>ConnectionStrings__DefaultConnection</c> directly)
/// are unaffected.
/// </remarks>
public static class DatabaseUrlExtensions
{
    private const string DatabaseUrlVariable = "DATABASE_URL";
    private const string DefaultConnectionConfigKey =
        "ConnectionStrings:" + DependencyInjection.ConnectionStringName;

    /// <summary>
    /// Reads <c>DATABASE_URL</c>, converts it to Npgsql syntax, and pushes
    /// it into the configuration under the canonical
    /// <c>ConnectionStrings:DefaultConnection</c> key.
    /// </summary>
    public static WebApplicationBuilder UsePlatformDatabaseUrl(this WebApplicationBuilder builder)
    {
        var url = Environment.GetEnvironmentVariable(DatabaseUrlVariable);
        if (string.IsNullOrWhiteSpace(url))
        {
            return builder;
        }

        builder.Configuration[DefaultConnectionConfigKey] = ConvertToNpgsqlConnectionString(url);
        return builder;
    }

    /// <summary>Pure helper, exposed internally so it can be unit-tested.</summary>
    internal static string ConvertToNpgsqlConnectionString(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = WebUtility.UrlDecode(userInfo[0]);
        var password = userInfo.Length > 1 ? WebUtility.UrlDecode(userInfo[1]) : string.Empty;
        var database = uri.AbsolutePath.TrimStart('/');
        var port = uri.IsDefaultPort ? 5432 : uri.Port;

        return $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
}
