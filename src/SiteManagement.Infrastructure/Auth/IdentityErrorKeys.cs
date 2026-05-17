namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Stable resource keys used when an ASP.NET Core Identity operation fails
/// (e.g. duplicate email, weak password, role assignment). The Application
/// layer's <c>BusinessRuleViolationException</c> carries these so the API
/// response can be reacted to programmatically.
/// </summary>
public static class IdentityErrorKeys
{
    /// <summary>User creation failed (duplicate email, password policy, etc.).</summary>
    public const string RegistrationFailed = "Identity.RegistrationFailed";

    /// <summary>Adding the user to the requested role failed.</summary>
    public const string RoleAssignmentFailed = "Identity.RoleAssignmentFailed";
}
