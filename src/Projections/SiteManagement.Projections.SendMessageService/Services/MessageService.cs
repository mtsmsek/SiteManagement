using SiteManagement.Application.Services.Messages;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Events.Messages;

namespace SiteManagement.Projections.SendMessageService.Services;

public class MessageService
{
    private readonly IMessageService _messageService;

    public MessageService(IMessageService messageService)//
    {
        _messageService = messageService;
    }

    public async Task SendMessage(SendMessageEvent @event, CancellationToken cancellationToken = default)
    {
        Message message = new()
        {
            CreatedDate = @event.SendedTime,
            SenderId = @event.SenderId,
            Text = @event.Message,
            ReceiverId = @event.ReceiverId
        };
        await _messageService.SendMessage(message, default);
    }
}
