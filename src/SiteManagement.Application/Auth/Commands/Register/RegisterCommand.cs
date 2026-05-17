using MediatR;

namespace SiteManagement.Application.Auth.Commands.Register;

/// <summary>
/// Registers an admin user. Resident creation has its own flow (W2) where the
/// admin creates the user and the system emails the generated password.
/// </summary>
public sealed record RegisterCommand(string Email, string Password, string FullName)
    : IRequest<RegisterResult>;

/// <summary>Result of a successful registration.</summary>
public sealed record RegisterResult(Guid UserId);
