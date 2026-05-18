using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;

namespace SiteManagement.Application.Auth.Commands.Login;

/// <summary>
/// Authenticates a user with email + password and mints a token pair.
/// Collapses every credential failure onto the same 401 message
/// (<see cref="ErrorMessageKeys.AuthInvalidCredentials"/>) to avoid user enumeration.
/// </summary>
public sealed class LoginCommandHandler(
    IUserAuthService userAuth,
    ITokenService tokenService,
    IRefreshTokenStore refreshTokenStore)
    : IRequestHandler<LoginCommand, AuthTokens>
{
    private readonly IUserAuthService _userAuth = userAuth;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokenStore _refreshTokenStore = refreshTokenStore;

    /// <inheritdoc />
    public async Task<AuthTokens> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userAuth.AuthenticateAsync(request.Email, request.Password, cancellationToken)
                   ?? throw new AuthenticationException(ErrorMessageKeys.AuthInvalidCredentials);

        var tokens = _tokenService.IssueTokens(user.Id, user.Email, user.Roles, user.ResidentId);

        await _refreshTokenStore.StoreAsync(
            user.Id,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAtUtc,
            cancellationToken);

        return tokens;
    }
}
