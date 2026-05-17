namespace SiteManagement.Domain.Identity;

/// <summary>
/// Stable role names and identifiers seeded once by the infrastructure layer
/// and referenced by <c>[Authorize(Roles = ...)]</c> attributes throughout the API.
/// </summary>
public static class Roles
{
    /// <summary>Role name for administrators (full site management capabilities).</summary>
    public const string Admin = "Admin";

    /// <summary>Role name for residents (read own dues, pay invoices, message admin).</summary>
    public const string Resident = "Resident";

    /// <summary>Stable seeded identifier for the <see cref="Admin"/> role.</summary>
    public static readonly Guid AdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>Stable seeded identifier for the <see cref="Resident"/> role.</summary>
    public static readonly Guid ResidentId = Guid.Parse("22222222-2222-2222-2222-222222222222");
}
