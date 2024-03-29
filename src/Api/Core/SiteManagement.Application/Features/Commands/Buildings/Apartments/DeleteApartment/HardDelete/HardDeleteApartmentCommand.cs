using MediatR;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete;

public class HardDeleteApartmentCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}
