using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;

public class UpdateResidentCommand : IRequest<UpdateResidentResponse>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public Guid ApartmentId { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get; set; }
}
