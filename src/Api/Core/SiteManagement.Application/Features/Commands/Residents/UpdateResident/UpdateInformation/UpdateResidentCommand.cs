using MediatR;
using SiteManagement.Application.Validators.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;

public class UpdateResidentCommand : IRequest<UpdateResidentResponse>, IResidentCommand
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get ; set; }
    public string Email { get; set; }
    public int BirthYear { get; set; }
    public int BirthMonth { get; set; }
    public int BirthDay { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get ; set ; }
}
