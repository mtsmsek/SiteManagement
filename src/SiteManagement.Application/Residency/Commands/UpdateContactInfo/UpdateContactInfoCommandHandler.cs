using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Residency.Commands.UpdateContactInfo;

/// <summary>
/// Replaces email + phone on an existing resident in one move. AppUser
/// email is left untouched here on purpose — that is a separate "change
/// login email" flow with its own confirmation requirements (deferred to W3).
/// </summary>
public sealed class UpdateContactInfoCommandHandler(
    IResidentRepository residentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateContactInfoCommand>
{
    private readonly IResidentRepository _residentRepository = residentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(UpdateContactInfoCommand request, CancellationToken cancellationToken)
    {
        var resident = await _residentRepository.GetByIdAsync(request.ResidentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Resident), request.ResidentId);

        resident.UpdateContactInfo(Email.From(request.Email), PhoneNumber.From(request.Phone));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
