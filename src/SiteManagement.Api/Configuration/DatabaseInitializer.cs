using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SiteManagement.Infrastructure.Auth;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;
using SiteManagement.Infrastructure.Persistence.Seed;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Runs at application startup: applies any pending EF Core migrations,
/// seeds the Admin/Resident roles, and (when configured) creates the
/// bootstrap admin user that the rest of the system is built on top of.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Migrates the database, seeds identity roles, and seeds the optional
    /// bootstrap admin user. Logs progress so a failed startup is easy to
    /// diagnose from container logs.
    /// </summary>
    public static async Task MigrateAndSeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseInitializer));

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        logger.LogInformation("Applying EF Core migrations...");
        await dbContext.Database.MigrateAsync(ct);

        logger.LogInformation("Seeding identity roles...");
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        await IdentitySeeder.SeedRolesAsync(roleManager, ct);

        logger.LogInformation("Seeding bootstrap admin (if configured)...");
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminBootstrapOptions>>().Value;
        await IdentitySeeder.SeedBootstrapAdminAsync(userManager, adminOptions, logger, ct);

        var demoOptions = scope.ServiceProvider.GetRequiredService<IOptions<DemoOptions>>().Value;
        if (demoOptions.SeedOnStartup)
        {
            logger.LogInformation("Demo seed flag is on — invoking DemoSeeder.");
            await new DemoSeeder().SeedAsync(services, logger, ct);
        }

        logger.LogInformation("Database initialization complete.");
    }
}
