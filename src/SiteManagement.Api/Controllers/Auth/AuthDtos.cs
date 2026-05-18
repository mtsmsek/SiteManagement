namespace SiteManagement.Api.Controllers.Auth;

/// <summary>Request body for <c>POST /api/auth/login</c>.</summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>Request body for <c>POST /api/auth/refresh</c>.</summary>
public sealed record RefreshRequest(string RefreshToken);

/// <summary>Response body shared by login and refresh endpoints.</summary>
public sealed record TokensResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
