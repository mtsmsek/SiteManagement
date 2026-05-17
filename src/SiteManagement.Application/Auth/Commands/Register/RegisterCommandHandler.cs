using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Application.Auth.Commands.Register;

/// <summary>
/// Creates a new <see cref="Roles.Admin"/> user via <see cref="IUserAuthService"/>.
/// Failure modes (duplicate email, weak password) surface as Identity errors
/// translated to <c>BusinessRuleViolationException</c> by the infrastructure layer.
/// </summary>
public sealed class RegisterCommandHandler(IUserAuthService userAuth)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IUserAuthService _userAuth = userAuth;

    /// <inheritdoc />
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userAuth.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName,
            Roles.Admin,
            cancellationToken);

        return new RegisterResult(userId);
    }
}
