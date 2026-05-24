using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Auth.Commands.Refresh;

/// <summary>Exchanges a valid refresh token for a new access + refresh token pair.</summary>
public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthTokens>;
