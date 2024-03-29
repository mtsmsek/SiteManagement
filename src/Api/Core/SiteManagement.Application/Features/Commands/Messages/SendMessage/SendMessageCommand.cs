using MediatR;

namespace SiteManagement.Application.Features.Commands.Messages.SendMessage
{
    public class SendMessageCommand : IRequest<SendMessageResponse>
    {
        public Guid ReceiverId { get; set; }
        public string Text { get; set; }
    }
}
