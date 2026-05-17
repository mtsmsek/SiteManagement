using Microsoft.AspNetCore.Identity;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Infrastructure.Identity;

namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Default <see cref="IUserAuthService"/> implementation backed by ASP.NET
/// Core Identity's <see cref="UserManager{TUser}"/>. Keeps the Application
/// layer free of any Identity-specific references.
/// </summary>
public class UserAuthService(UserManager<AppUser> userManager) : IUserAuthService
{
    private readonly UserManager<AppUser> _userManager = userManager;

    /// <inheritdoc />
    public async Task<Guid> RegisterAsync(
        string email,
        string password,
        string fullName,
        string role,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            throw new BusinessRuleViolationException(
                FormatIdentityErrors(createResult),
                IdentityErrorKeys.RegistrationFailed);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            throw new BusinessRuleViolationException(
                FormatIdentityErrors(roleResult),
                IdentityErrorKeys.RoleAssignmentFailed);
        }

        return user.Id;
    }

    /// <inheritdoc />
    public async Task<AuthenticatedUser?> AuthenticateAsync(string email, string password, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var passwordOk = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordOk)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new AuthenticatedUser(user.Id, user.Email!, user.FullName, roles.ToArray());
    }

    /// <inheritdoc />
    public async Task<AuthenticatedUser?> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new AuthenticatedUser(user.Id, user.Email!, user.FullName, roles.ToArray());
    }

    /// <summary>Flattens an <see cref="IdentityResult"/> error collection into a single line.</summary>
    private static string FormatIdentityErrors(IdentityResult result)
        => string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
}
