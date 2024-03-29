using MediatR;

namespace SiteManagement.Application.Features.Commands.Residents.CreateResident;

public class CreateResidentCommand : IRequest<CreateResidentResponse>
{
    public Guid ApartmentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get; set; }
}
