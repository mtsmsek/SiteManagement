using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Email;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Residency.Commands.RegisterResident;

/// <summary>
/// Admin-driven flow. Creates the Domain <see cref="Resident"/> aggregate
/// plus the linked Identity <c>AppUser</c> as a single atomic transaction,
/// then emails the resident the generated password.
/// </summary>
/// <remarks>
/// Strict atomicity: the welcome email is sent <em>inside</em> the
/// transaction. If SMTP fails the scope disposes uncommitted and Postgres
/// rolls back both inserts — because the system is the only party that
/// knows the random password, a resident the system can't notify is
/// effectively unusable. The handler is therefore safe to retry: a
/// failed run leaves nothing behind.
/// </remarks>
public sealed class RegisterResidentCommandHandler(
    IResidentRepository residentRepository,
    IUnitOfWork unitOfWork,
    IUserAuthService userAuth,
    IPasswordGenerator passwordGenerator,
    IEmailSender emailSender)
    : IRequestHandler<RegisterResidentCommand, RegisterResidentResult>
{
    private readonly IResidentRepository _residentRepository = residentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserAuthService _userAuth = userAuth;
    private readonly IPasswordGenerator _passwordGenerator = passwordGenerator;
    private readonly IEmailSender _emailSender = emailSender;

    /// <inheritdoc />
    public async Task<RegisterResidentResult> Handle(RegisterResidentCommand request, CancellationToken cancellationToken)
    {
        var tcNo = TcNo.From(request.TcNo);
        var fullName = FullName.Create(request.FirstName, request.LastName);
        var email = Email.From(request.Email);
        var phone = PhoneNumber.From(request.Phone);

        var existing = await _residentRepository.FindByTcNoAsync(tcNo, cancellationToken);
        if (existing is not null)
        {
            throw new BusinessRuleViolationException(
                ErrorMessageKeys.ResidencyDuplicateTcNo,
                ErrorMessageKeys.ResidencyDuplicateTcNo);
        }

        var resident = Resident.Create(tcNo, fullName, email, phone);
        var password = _passwordGenerator.Generate();
        var displayName = fullName.ToString();

        // TransactionBehavior wraps this command in a scope, so the inserts +
        // the welcome email all run inside one transaction (see remarks).
        await _residentRepository.AddAsync(resident, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _userAuth.RegisterResidentUserAsync(
            resident.Id,
            email.Value,
            password,
            displayName,
            cancellationToken);

        // Inside the transaction on purpose — see remarks.
        await _emailSender.SendResidentWelcomeAsync(email.Value, displayName, password, cancellationToken);

        return new RegisterResidentResult(resident.Id);
    }
}
