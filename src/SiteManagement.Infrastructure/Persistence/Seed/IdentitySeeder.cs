using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SiteManagement.Domain.Identity;
using SiteManagement.Infrastructure.Auth;
using SiteManagement.Infrastructure.Identity;

namespace SiteManagement.Infrastructure.Persistence.Seed;

/// <summary>
/// Startup-time seed helpers for Identity-side data: the Admin/Resident
/// roles, and the optional bootstrap admin user read from configuration.
/// </summary>
public static class IdentitySeeder
{
    /// <summary>Idempotently seeds the two stable roles (Admin, Resident).</summary>
    public static async Task SeedRolesAsync(RoleManager<AppRole> roleManager, CancellationToken ct = default)
    {
        await EnsureRoleAsync(roleManager, Roles.AdminId, Roles.Admin, ct);
        await EnsureRoleAsync(roleManager, Roles.ResidentId, Roles.Resident, ct);
    }

    /// <summary>
    /// Seeds the bootstrap admin user described by
    /// <paramref name="options"/>, if any. Idempotent: when an account with
    /// the same email already exists the method returns without touching it
    /// (no password reset, no role re-assignment).
    /// </summary>
    /// <remarks>
    /// This is the only path a brand-new deployment can use to obtain its
    /// first admin. After it logs in once, all subsequent admin / resident
    /// accounts are created through authenticated endpoints — there is no
    /// public self-service registration on this API.
    /// </remarks>
    public static async Task SeedBootstrapAdminAsync(
        UserManager<AppUser> userManager,
        AdminBootstrapOptions options,
        ILogger logger,
        CancellationToken ct = default)
    {
        if (!options.IsConfigured)
        {
            logger.LogInformation("Bootstrap admin not configured (Admin section missing); skipping seed.");
            return;
        }

        var email = options.BootstrapEmail!;
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            logger.LogInformation("Bootstrap admin {Email} already exists; leaving untouched.", email);
            return;
        }

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = options.BootstrapFullName,
            EmailConfirmed = true,
            ResidentId = null,
        };

        var createResult = await userManager.CreateAsync(user, options.BootstrapPassword!);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed bootstrap admin '{email}': {FormatErrors(createResult)}");
        }

        var roleResult = await userManager.AddToRoleAsync(user, Roles.Admin);
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to add bootstrap admin '{email}' to Admin role: {FormatErrors(roleResult)}");
        }

        logger.LogInformation("Seeded bootstrap admin {Email}.", email);
    }

    private static async Task EnsureRoleAsync(
        RoleManager<AppRole> roleManager,
        Guid id,
        string name,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var existing = await roleManager.FindByIdAsync(id.ToString());
        if (existing is not null)
        {
            return;
        }

        var role = new AppRole(name) { Id = id };
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed role '{name}': {FormatErrors(result)}");
        }
    }

    private static string FormatErrors(IdentityResult result)
        => string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
}
