using MediatR;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail
{
    public class UpdateResidentEmailCommand : IRequest<UpdateResidentEmailResponse>
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
