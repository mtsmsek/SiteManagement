using MediatR;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Auth.Commands.Login;
using SiteManagement.Application.Auth.Commands.Refresh;

namespace SiteManagement.Api.Controllers.Auth;

/// <summary>
/// Authentication endpoints. Thin controller that translates HTTP DTOs to
/// MediatR commands; all business logic lives in the handlers.
/// </summary>
/// <remarks>
/// Deliberately no <c>register</c> endpoint here. The bootstrap admin is
/// seeded at startup from environment variables; every subsequent user
/// (resident or admin) is created by an authenticated admin via the
/// resident / admin management endpoints, never by anonymous self-service.
/// </remarks>
[ApiController]
[Route($"{ApiConstants.RoutePrefix}/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    private const string LoginRoute = "login";
    private const string RefreshRoute = "refresh";

    private readonly ISender _sender = sender;

    /// <summary>Authenticates an existing user and returns an access + refresh token pair.</summary>
    [HttpPost(LoginRoute)]
    [ProducesResponseType<TokensResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokensResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var tokens = await _sender.Send(new LoginCommand(request.Email, request.Password), ct);

        return Ok(new TokensResponse(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAtUtc,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAtUtc));
    }

    /// <summary>Rotates a refresh token, returning a fresh access + refresh pair.</summary>
    [HttpPost(RefreshRoute)]
    [ProducesResponseType<TokensResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokensResponse>> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken ct)
    {
        var tokens = await _sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        return Ok(new TokensResponse(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAtUtc,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAtUtc));
    }
}
