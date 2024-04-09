using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SiteManagement.Application.Messaging;
using SiteManagement.Application.Security.Extensions;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Events.Messages;

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
           // var senderId = _httpContextAccessor.HttpContext.User.GetUserId();
           
            //TODO -- Add validation
           // var messageToSend = _mapper.Map<Message>(request);
            //messageToSend.SenderId = senderId;7
            
            //todo -- remove magic string
            QueueFactory.SendMessageToExchange(exchangeName: "MessageExchange",
                                                exchangeType: "direct",
                                                queueName: "SendMessageQueue",
                                                obj:new SendMessageEvent
                                                {
                                                    SenderId = request.SenderId,
                                                    Message = request.Text,
                                                    SendedTime = DateTime.Now,
                                                    ReceiverId = request.ReceiverId
                                                    
                                                });

            //todo -- remove ???
            // await _messageRepository.AddAsync(messageToSend, cancellationToken);

            // return _mapper.Map<SendMessageResponse>(messageToSend);
            return new SendMessageResponse();

        }
    }
}
