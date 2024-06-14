using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.VehicleResident.DeleteVehicleResident.HardDelete
{
    public class HardDeleteResidentVehicleCommand : IRequest<int>
    {
        public Guid ResidentVehicleId { get; set; }
    }
}
