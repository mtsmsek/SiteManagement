using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Residency.Commands.RemoveVehicle;

/// <summary>
/// Delegates to <see cref="Resident.RemoveVehicle"/>; domain throws
/// <c>VehicleNotFoundException</c> when the plate is not registered on
/// this resident.
/// </summary>
public sealed class RemoveVehicleCommandHandler(
    IResidentRepository residentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveVehicleCommand>
{
    private readonly IResidentRepository _residentRepository = residentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(RemoveVehicleCommand request, CancellationToken cancellationToken)
    {
        var resident = await _residentRepository.GetByIdAsync(request.ResidentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Resident), request.ResidentId);

        resident.RemoveVehicle(PlateNumber.From(request.Plate));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
