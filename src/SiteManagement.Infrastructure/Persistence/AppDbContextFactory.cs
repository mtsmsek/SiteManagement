using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SiteManagement.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations` resolve a DbContext without booting the API.
/// Reads EF_CONNECTION_STRING when set, falls back to the local docker-compose
/// default so `dotnet ef migrations add ...` works out of the box.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=sitemanagement;Username=sitemanagement;Password=sitemanagement_dev_pw";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("EF_CONNECTION_STRING")
                               ?? DefaultConnectionString;

        var builder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString, npg => npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        return new AppDbContext(builder.Options);
    }
}
