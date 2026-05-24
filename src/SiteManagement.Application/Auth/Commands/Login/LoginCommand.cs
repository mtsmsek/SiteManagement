using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Auth.Commands.Login;

/// <summary>Validates credentials and returns a fresh access + refresh token pair.</summary>
public sealed record LoginCommand(string Email, string Password) : ICommand<AuthTokens>;
