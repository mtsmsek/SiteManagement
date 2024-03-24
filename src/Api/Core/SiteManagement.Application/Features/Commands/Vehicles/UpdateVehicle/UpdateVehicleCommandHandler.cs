using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle
{
    public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, UpdateVehicleCommandResponse>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;
        private readonly VehicleBusinessRules _vehicleBusinessRules;

        public UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository, IMapper mapper, VehicleBusinessRules vehicleBusinessRules)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _vehicleBusinessRules = vehicleBusinessRules;
        }

        public async Task<UpdateVehicleCommandResponse> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            Vehicle vehicle = await _vehicleBusinessRules.CheckIfVehiceExistById(request.Id, cancellationToken);    

            _mapper.Map(request, vehicle);

            await _vehicleRepository.UpdateAsync(vehicle);
            return _mapper.Map<UpdateVehicleCommandResponse>(vehicle);
        }
    }
}
