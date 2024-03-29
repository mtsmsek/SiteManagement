using MediatR;

namespace SiteManagement.Application.Features.Commands.Residents.DeleteResident.HardDelete;

public class HardDeleteResidentCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}
