using MediatR;
using SiteManagement.Application.Validators.Residents;

namespace SiteManagement.Application.Features.Commands.Residents.CreateResident;

public class CreateResidentCommand : IRequest<CreateResidentResponse>, IResidentCommand
{
    public Guid ApartmentId { get; set; }
    public string FirstName { get ; set; }
    public string LastName { get ; set ; }
    public string Email { get ; set; }
    public int BirthYear { get; set; }
    public int BirthMonth { get; set; }
    public int BirthDay { get; set; }
    public string IdenticalNumber { get ; set ; }
    public string PhoneNumber { get; set; }
}
