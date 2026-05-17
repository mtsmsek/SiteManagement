using MediatR;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Auth.Commands.Login;
using SiteManagement.Application.Auth.Commands.Refresh;
using SiteManagement.Application.Auth.Commands.Register;

namespace SiteManagement.Api.Controllers.Auth;

/// <summary>
/// Authentication endpoints. The controller is a thin shell that translates
/// HTTP DTOs to MediatR commands; all business logic lives in the handlers.
/// </summary>
[ApiController]
[Route($"{ApiConstants.RoutePrefix}/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    private const string RegisterRoute = "register";
    private const string LoginRoute = "login";
    private const string RefreshRoute = "refresh";

    private readonly ISender _sender = sender;

    /// <summary>Creates an admin user. Resident accounts use a different flow (W2).</summary>
    [HttpPost(RegisterRoute)]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RegisterCommand(request.Email, request.Password, request.FullName),
            ct);

        return Ok(new RegisterResponse(result.UserId));
    }

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
