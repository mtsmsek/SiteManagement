using Microsoft.AspNetCore.Identity;

namespace SiteManagement.Infrastructure.Identity;

/// <summary>
/// ASP.NET Core Identity user. Lives in the Infrastructure layer because
/// <see cref="IdentityUser{TKey}"/> is a persistence concern; the actual
/// Resident aggregate (with domain invariants) sits in the Domain layer and
/// is linked from W2 onward via <see cref="ResidentId"/>.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>Display name shown in the UI; also embedded in the JWT.</summary>
    public string FullName { get; set; } = string.Empty;
}
