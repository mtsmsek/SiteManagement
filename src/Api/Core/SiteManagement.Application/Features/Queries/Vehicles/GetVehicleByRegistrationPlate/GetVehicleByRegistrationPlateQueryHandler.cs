﻿using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;

namespace SiteManagement.Application.Features.Queries.Vehicles.GetVehicleByRegistrationPlate
{
    public class GetVehicleByRegistrationPlateQueryHandler : IRequestHandler<GetVehicleByRegistrationPlateQuery, GetVehicleByRegistrationPlateResponse>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public GetVehicleByRegistrationPlateQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<GetVehicleByRegistrationPlateResponse> Handle(GetVehicleByRegistrationPlateQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await _vehicleRepository.GetSingleAsync(predicate: vehicle => vehicle.VehicleRegistrationPlate == request.VehicleRegistrationPlate);

            //todo -- move it to vehicle vusiness rules ??
            if (vehicle is null)
                throw new BusinessException(VehicleMessages.RuleMessages.NoVehicleFoundBelongToIndicatedRegistrationPlate);


            return _mapper.Map<GetVehicleByRegistrationPlateResponse>(vehicle);
        }

    }
}
