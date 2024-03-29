using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SiteManagement.Application.Security.Extensions;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;

namespace SiteManagement.Application.Features.Commands.Messages.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendMessageCommandHandler(IMessageRepository messageRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            
            //todo-- check that => 1) take id with queryString   2) take the id command class => set it in controller 3) this solution
            var senderId = _httpContextAccessor.HttpContext.User.GetUserId();
           
            //TODO -- Add validation
            var messageToSend = _mapper.Map<Message>(request);
            messageToSend.SenderId = senderId;
            await _messageRepository.AddAsync(messageToSend, cancellationToken);
            //todo send message to queue
            return _mapper.Map<SendMessageResponse>(messageToSend);
        }
    }
}
