using MediatR;
using SiteManagement.Application.Abstractions.Auth;

namespace SiteManagement.Application.Auth.Commands.Login;

/// <summary>Validates credentials and returns a fresh access + refresh token pair.</summary>
public sealed record LoginCommand(string Email, string Password) : IRequest<AuthTokens>;
