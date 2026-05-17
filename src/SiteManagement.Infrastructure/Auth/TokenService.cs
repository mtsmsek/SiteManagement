using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SiteManagement.Application.Abstractions.Auth;

namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Default <see cref="ITokenService"/> implementation. Issues HS256-signed
/// JWT access tokens and cryptographically random refresh tokens.
/// </summary>
public class TokenService(IOptions<JwtOptions> options, TimeProvider clock) : ITokenService
{
    private readonly JwtOptions _options = options.Value;
    private readonly TimeProvider _clock = clock;

    /// <inheritdoc />
    public AuthTokens IssueTokens(Guid userId, string email, IEnumerable<string> roles)
    {
        var nowUtc = _clock.GetUtcNow().UtcDateTime;
        var accessExpiresAt = nowUtc.AddMinutes(_options.AccessTokenMinutes);
        var refreshExpiresAt = nowUtc.AddDays(_options.RefreshTokenDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: accessExpiresAt,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new AuthTokens(
            AccessToken: accessToken,
            AccessTokenExpiresAtUtc: accessExpiresAt,
            RefreshToken: GenerateRefreshToken(),
            RefreshTokenExpiresAtUtc: refreshExpiresAt);
    }

    /// <summary>
    /// Generates a 64-byte cryptographically random refresh token, base64-encoded.
    /// </summary>
    private static string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
