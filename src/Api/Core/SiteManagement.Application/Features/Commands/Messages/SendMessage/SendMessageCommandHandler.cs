using AutoMapper;
using MediatR;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;

namespace SiteManagement.Application.Features.Commands.Messages.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public SendMessageCommandHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            //TODO -- Add validation
            var messageToSend = _mapper.Map<Message>(request);

            await _messageRepository.AddAsync(messageToSend, cancellationToken);

            return _mapper.Map<SendMessageResponse>(messageToSend);
        }
    }
}
