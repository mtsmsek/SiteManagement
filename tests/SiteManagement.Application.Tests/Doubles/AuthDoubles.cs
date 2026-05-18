using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Application.Tests.Doubles;

/// <summary>
/// Shared factory + sample-value constants for auth-related test data.
/// Keeping construction here means a signature change in
/// <see cref="AuthenticatedUser"/> or <see cref="AuthTokens"/> only needs
/// one update across the test suite.
/// </summary>
public static class AuthDoubles
{
    /// <summary>Default email used by handler tests.</summary>
    public const string DefaultEmail = "admin@example.com";

    /// <summary>Default password used by handler tests (passes <c>ValidPassword</c> rule).</summary>
    public const string DefaultPassword = "Str0ng-P@ss";

    /// <summary>Default full name used by handler tests.</summary>
    public const string DefaultFullName = "Default Admin";

    /// <summary>Fake access token string used in handler tests.</summary>
    public const string SampleAccessToken = "fake-access-token";

    /// <summary>Fake refresh token string used in handler tests.</summary>
    public const string SampleRefreshToken = "fake-refresh-token";

    /// <summary>Creates a sample <see cref="AuthenticatedUser"/> with sensible defaults.</summary>
    public static AuthenticatedUser SampleUser(
        Guid? id = null,
        string email = DefaultEmail,
        string fullName = DefaultFullName,
        IReadOnlyList<string>? roles = null,
        Guid? residentId = null)
        => new(id ?? Guid.NewGuid(), email, fullName, roles ?? new[] { Roles.Admin }, residentId);

    /// <summary>Creates a sample <see cref="AuthTokens"/> with sensible expiry defaults.</summary>
    public static AuthTokens SampleTokens(
        string accessToken = SampleAccessToken,
        string refreshToken = SampleRefreshToken,
        DateTime? accessExpiresAtUtc = null,
        DateTime? refreshExpiresAtUtc = null)
        => new(
            accessToken,
            accessExpiresAtUtc ?? DateTime.UtcNow.AddHours(1),
            refreshToken,
            refreshExpiresAtUtc ?? DateTime.UtcNow.AddDays(14));
}
