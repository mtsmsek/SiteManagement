using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;
using SiteManagement.Infrastructure.Persistence.Seed;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Runs at application startup: applies any pending EF Core migrations and
/// seeds the Admin/Resident roles. Keeps <c>Program.cs</c> small.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Migrates the database and seeds identity roles. Logs progress so a
    /// failed startup is easy to diagnose from container logs.
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

        logger.LogInformation("Database initialization complete.");
    }
}
