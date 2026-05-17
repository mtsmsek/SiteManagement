using Microsoft.AspNetCore.Identity;

namespace SiteManagement.Infrastructure.Identity;

/// <summary>
/// ASP.NET Core Identity role. Two are seeded at startup
/// (<see cref="Domain.Identity.Roles.Admin"/> and
/// <see cref="Domain.Identity.Roles.Resident"/>) with stable Guids so the
/// roles table is identical across environments and tests.
/// </summary>
public class AppRole : IdentityRole<Guid>
{
    /// <summary>Required parameterless ctor used by EF Core / Identity.</summary>
    public AppRole() { }

    /// <summary>Creates a role with the given name and a derived normalized name.</summary>
    /// <param name="name">Display name; will be uppercased for the lookup column.</param>
    public AppRole(string name) : base(name)
    {
        NormalizedName = name.ToUpperInvariant();
    }
}
