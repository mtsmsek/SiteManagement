using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Residency.Commands.AddVehicle;

/// <summary>
/// Delegates the duplicate-plate invariant to the
/// <see cref="Resident.AddVehicle"/> entity method; the domain throws
/// <c>DuplicateVehiclePlateException</c> on a clash and the MediatR
/// pipeline turns it into a localized 409.
/// </summary>
public sealed class AddVehicleCommandHandler(
    IResidentRepository residentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddVehicleCommand>
{
    private readonly IResidentRepository _residentRepository = residentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        var resident = await _residentRepository.GetByIdAsync(request.ResidentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Resident), request.ResidentId);

        var vehicle = VehicleInfo.Create(PlateNumber.From(request.Plate), request.Note);
        resident.AddVehicle(vehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
