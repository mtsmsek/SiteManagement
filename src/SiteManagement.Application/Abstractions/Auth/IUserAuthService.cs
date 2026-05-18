namespace SiteManagement.Application.Abstractions.Auth;

/// <summary>
/// Application-facing port over ASP.NET Core Identity. Keeps the Application
/// layer free of <c>UserManager</c>/<c>SignInManager</c> references so it
/// stays infrastructure-agnostic and easy to unit-test with NSubstitute.
/// </summary>
/// <remarks>
/// The admin and resident registration flows are split into separate
/// methods on purpose: the resident flow takes a non-nullable
/// <c>residentId</c> tying the AppUser to the Domain aggregate, and the
/// admin flow forbids supplying one. Caller can't accidentally mix them up.
/// </remarks>
public interface IUserAuthService
{
    /// <summary>Creates an Admin user with no resident link.</summary>
    /// <returns>The new user's identifier on success.</returns>
    Task<Guid> RegisterAdminAsync(
        string email,
        string password,
        string fullName,
        CancellationToken ct);

    /// <summary>Creates a Resident user linked to the given Domain Resident aggregate.</summary>
    /// <returns>The new user's identifier on success.</returns>
    Task<Guid> RegisterResidentUserAsync(
        Guid residentId,
        string email,
        string password,
        string fullName,
        CancellationToken ct);

    /// <summary>Validates credentials and returns the principal payload needed to mint tokens.</summary>
    /// <returns><c>null</c> if the user is not found or the password is wrong.</returns>
    Task<AuthenticatedUser?> AuthenticateAsync(string email, string password, CancellationToken ct);

    /// <summary>Loads a user by id; returns <c>null</c> if missing.</summary>
    Task<AuthenticatedUser?> GetByIdAsync(Guid userId, CancellationToken ct);
}

/// <summary>
/// Minimal user projection needed by the auth commands when issuing tokens.
/// Carries <see cref="ResidentId"/> when the principal is a resident so the
/// token service can embed it as the <c>resident_id</c> claim.
/// </summary>
public sealed record AuthenticatedUser(
    Guid Id,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles,
    Guid? ResidentId);
