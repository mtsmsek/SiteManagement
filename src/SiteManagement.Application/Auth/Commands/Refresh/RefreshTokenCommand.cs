using MediatR;
using SiteManagement.Application.Abstractions.Auth;

namespace SiteManagement.Application.Auth.Commands.Refresh;

/// <summary>Exchanges a valid refresh token for a new access + refresh token pair.</summary>
public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthTokens>;
