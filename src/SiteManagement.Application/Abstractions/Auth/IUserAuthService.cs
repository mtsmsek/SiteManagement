namespace SiteManagement.Application.Abstractions.Auth;

/// <summary>
/// Application-facing port over ASP.NET Core Identity. Keeps the Application
/// layer free of <c>UserManager</c>/<c>SignInManager</c> references so it
/// stays infrastructure-agnostic and easy to unit-test with NSubstitute.
/// </summary>
public interface IUserAuthService
{
    /// <summary>Creates a new user with the supplied details and assigns the given role.</summary>
    /// <returns>The new user's identifier on success.</returns>
    Task<Guid> RegisterAsync(
        string email,
        string password,
        string fullName,
        string role,
        CancellationToken ct);

    /// <summary>Validates credentials and returns the principal payload needed to mint tokens.</summary>
    /// <returns><c>null</c> if the user is not found or the password is wrong.</returns>
    Task<AuthenticatedUser?> AuthenticateAsync(string email, string password, CancellationToken ct);

    /// <summary>Loads a user by id; returns <c>null</c> if missing.</summary>
    Task<AuthenticatedUser?> GetByIdAsync(Guid userId, CancellationToken ct);
}

/// <summary>
/// Minimal user projection needed by the auth commands when issuing tokens.
/// </summary>
public sealed record AuthenticatedUser(Guid Id, string Email, string FullName, IReadOnlyList<string> Roles);
