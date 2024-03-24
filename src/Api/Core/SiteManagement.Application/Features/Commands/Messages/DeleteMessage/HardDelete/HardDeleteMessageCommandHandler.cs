using MediatR;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Commands.Messages.DeleteMessage.HardDelete;

public class HardDeleteMessageCommandHandler : IRequestHandler<HardDeleteMessageCommand, int>
{
    private readonly IMessageRepository _messageRepository;

    public HardDeleteMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<int> Handle(HardDeleteMessageCommand request, CancellationToken cancellationToken)
    {
        //TODO -- add this function to business rules and throw exception in case of not found messaaage
        var messageToDelete = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken: cancellationToken);

       return await _messageRepository.DeleteAsync(messageToDelete, isPermenant: true, cancellationToken: cancellationToken);

        //TODO -- update message ?? 
    }
}