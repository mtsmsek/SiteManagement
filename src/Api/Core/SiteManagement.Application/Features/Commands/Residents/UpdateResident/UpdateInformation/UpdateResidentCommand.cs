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
    public string FirstName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string LastName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Email { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int BirthYear { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int BirthMonth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int BirthDay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string IdenticalNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string PhoneNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
