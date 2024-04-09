using SiteManagement.Application.Services.Messages;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;

namespace SiteManagement.Persistance.Services.Messages;

public class MessageManager : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageManager(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task SendMessage(Message message, CancellationToken cancellationToken)
    {
        await _messageRepository.AddAsync(message);
    }
}
