using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus
{
    public class ChangeResidentStatusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
    }
}
