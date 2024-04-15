using MediatR;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.DeleteBlock.HardDelete;

public class HardDeleteBlockCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

}
