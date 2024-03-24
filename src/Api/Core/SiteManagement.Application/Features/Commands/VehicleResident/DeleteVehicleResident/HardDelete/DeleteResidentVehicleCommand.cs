using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.VehicleResident.DeleteVehicleResident.HardDelete
{
    public class DeleteResidentVehicleCommand : IRequest<int>
    {
        public Guid ResidentId { get; set; }
    }
}
