﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.VehicleResident.UpdateVehicleResident
{
    public class UpdateResidentVehicleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid VehicleId { get; set; }

    }
}
