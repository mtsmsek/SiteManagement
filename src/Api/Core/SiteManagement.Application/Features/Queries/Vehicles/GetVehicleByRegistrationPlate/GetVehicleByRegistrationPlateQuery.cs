using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Vehicles.GetVehicleByRegistrationPlate
{
    public class GetVehicleByRegistrationPlateQuery : IRequest<GetVehicleByRegistrationPlateResponse>
    {
        public string VehicleRegistrationPlate{ get; set; }
    }
}
