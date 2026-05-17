using Microsoft.AspNetCore.Identity;
using SiteManagement.Domain.Identity;
using SiteManagement.Infrastructure.Identity;

namespace SiteManagement.Infrastructure.Persistence.Seed;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<AppRole> roleManager, CancellationToken ct = default)
    {
        await EnsureRoleAsync(roleManager, Roles.AdminId, Roles.Admin, ct);
        await EnsureRoleAsync(roleManager, Roles.ResidentId, Roles.Resident, ct);
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
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException($"Failed to seed role '{name}': {errors}");
        }
    }
}
