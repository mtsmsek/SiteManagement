using MediatR;

namespace SiteManagement.Application.Features.Commands.Messages.SendMessage
{
    public class SendMessageCommand : IRequest<SendMessageResponse>
    {
        public string Text { get; set; }
    }
}
