using SiteManagement.Domain.Entities.Residents;

namespace SiteManagement.Application.Services.Messages;

public interface IMessageService
{
    public Task SendMessage(Message message, CancellationToken cancellationToken);
}
