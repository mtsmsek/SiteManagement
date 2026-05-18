using Microsoft.AspNetCore.Identity;

namespace SiteManagement.Infrastructure.Identity;

/// <summary>
/// ASP.NET Core Identity user. Lives in the Infrastructure layer because
/// <see cref="IdentityUser{TKey}"/> is a persistence concern; the business
/// identity of a "resident" belongs to the Domain layer and is linked from
/// here via <see cref="ResidentId"/>.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>Display name shown in the UI; also embedded in the JWT.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the Domain <c>Resident</c> aggregate. <c>null</c> for
    /// admins (an admin is not a resident); set to <c>Resident.Id</c> for
    /// users created through <c>IUserAuthService.RegisterResidentUserAsync</c>.
    /// Emitted as the <c>resident_id</c> claim on access tokens so
    /// resident-only endpoints resolve the current resident without a join.
    /// </summary>
    public Guid? ResidentId { get; set; }
}
