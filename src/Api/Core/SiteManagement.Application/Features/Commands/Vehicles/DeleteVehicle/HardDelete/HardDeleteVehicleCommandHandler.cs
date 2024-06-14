using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete;
using SiteManagement.Application.Features.Commands.Vehicles.DeleteCehicle.HardDelete;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Vehicles.DeleteVehicle.HardDelete;

public class HardDeleteVehicleCommandHandler : IRequestHandler<HardDeleteVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;

    public HardDeleteVehicleCommandHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<int> Handle(HardDeleteVehicleCommand request, CancellationToken cancellationToken)
    {
       var vehicleToDelete =  await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken: cancellationToken);
        //TODO --this control in => vehicle business rule ??
       
        if (vehicleToDelete is null)
            throw new BusinessException(VehicleMessages.RuleMessages.VehicleCannotFound);

        return await _vehicleRepository.DeleteAsync(vehicleToDelete,
                                             isPermenant: true,
                                             cancellationToken: cancellationToken);

        
    }
}
