using MediatR;

namespace SiteManagement.Application.Features.Commands.Messages.DeleteMessage.HardDelete
{
    public class HardDeleteMessageCommand : IRequest<int>
    {
        public Guid MessageId { get; set; }

    }
}
