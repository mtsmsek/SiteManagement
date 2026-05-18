using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;

namespace SiteManagement.Application.Auth.Commands.Refresh;

/// <summary>
/// Consumes the supplied refresh token, loads the owning user, and issues a
/// fresh token pair. Throws <see cref="AuthenticationException"/> (401) when
/// the token is unknown, expired, or already consumed.
/// </summary>
public sealed class RefreshTokenCommandHandler(
    IRefreshTokenStore refreshTokenStore,
    IUserAuthService userAuth,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, AuthTokens>
{
    private readonly IRefreshTokenStore _refreshTokenStore = refreshTokenStore;
    private readonly IUserAuthService _userAuth = userAuth;
    private readonly ITokenService _tokenService = tokenService;

    /// <inheritdoc />
    public async Task<AuthTokens> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = await _refreshTokenStore.ConsumeAsync(request.RefreshToken, cancellationToken)
                     ?? throw new AuthenticationException(ErrorMessageKeys.AuthRefreshTokenInvalid);

        var user = await _userAuth.GetByIdAsync(userId, cancellationToken)
                   ?? throw new AuthenticationException(ErrorMessageKeys.AuthRefreshOwnerMissing);

        var tokens = _tokenService.IssueTokens(user.Id, user.Email, user.Roles, user.ResidentId);

        await _refreshTokenStore.StoreAsync(
            user.Id,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAtUtc,
            cancellationToken);

        return tokens;
    }
}
