using MediatR;
using SiteManagement.Application.Validators.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle
{
    public class UpdateVehicleCommand : IRequest<UpdateVehicleCommandResponse>, IVehicleCommand
    {
        public Guid Id { get; set; }
        public string VehicleRegistrationPlate { get; set; }
        public int VehicleType { get; set; }
    }
}
