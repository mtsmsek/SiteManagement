namespace SiteManagement.Application.Shared.Resources;

/// <summary>
/// Stable resource keys used by <see cref="ErrorMessages"/> lookups via
/// <c>IStringLocalizer&lt;ErrorMessages&gt;</c>. Every domain or application
/// error that surfaces to the client maps to one of these so clients (and
/// tests) can react to the failure programmatically regardless of the
/// active culture.
/// </summary>
public static class ErrorMessageKeys
{
    /// <summary>Generic fallback when no specific key fits.</summary>
    public const string Generic = "Error.Generic";

    /// <summary>Authentication failed: wrong email/password or account not found.</summary>
    public const string AuthInvalidCredentials = "Auth.InvalidCredentials";

    /// <summary>Refresh token is unknown, expired, or already consumed.</summary>
    public const string AuthRefreshTokenInvalid = "Auth.RefreshTokenInvalid";

    /// <summary>The user owning a refresh token no longer exists.</summary>
    public const string AuthRefreshOwnerMissing = "Auth.RefreshOwnerMissing";

    /// <summary>Identity could not create the user (duplicate email, weak password, ...).</summary>
    public const string AuthRegistrationFailed = "Auth.RegistrationFailed";

    /// <summary>Role assignment failed after the user was created.</summary>
    public const string AuthRoleAssignmentFailed = "Auth.RoleAssignmentFailed";

    /// <summary>RFC 7807 default title for the validation problem.</summary>
    public const string ValidationFailed = "Validation.Failed";

    /// <summary>Resident registration attempted with a TcNo that is already on file.</summary>
    public const string ResidencyDuplicateTcNo = "Residency.DuplicateTcNo";

    /// <summary>Resident registration attempted with an email that is already on file (Identity unique constraint).</summary>
    public const string ResidencyDuplicateEmail = "Residency.DuplicateEmail";

    /// <summary>Assignment attempted on an apartment that already has an active occupant.</summary>
    public const string TenancyApartmentAlreadyAssigned = "Tenancy.Apartment.AlreadyAssigned";
}
