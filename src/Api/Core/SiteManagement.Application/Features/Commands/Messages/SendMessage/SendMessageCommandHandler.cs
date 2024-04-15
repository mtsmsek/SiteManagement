using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Messaging;
using SiteManagement.Application.Pipelines.Transaction;
using SiteManagement.Application.Pipelines.Authorization;
using SiteManagement.Application.Security.Extensions;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Events.Messages;
using static SiteManagement.Domain.Constants.Security.UsersOperationClaims;


namespace SiteManagement.Application.Features.Commands.Messages.SendMessage
{

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>, ISecuredRequest, ITransactionalRequest

    {
        private readonly IMapper _mapper;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendMessageCommandHandler(IMessageRepository messageRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserOperationClaimRepository userOperationClaimRepository)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userOperationClaimRepository = userOperationClaimRepository;
        }

        public string[] Roles => [Admin,User];

        public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            //todo --remove magic strings
            var senderId = _httpContextAccessor.HttpContext.User.GetUserId();
            
            var operationClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();

            if (request.SenderId == request.ReceiverId)
                throw new BusinessException("Kendinize mesaj gönderemezsiniz");

            if (!operationClaims!.Contains(Admin))
            {
                var receiverOperationClaims = await _userOperationClaimRepository.GetListAsync(predicate: u => u.UserId == request.ReceiverId,
                                                                                cancellationToken: cancellationToken,
                                                                                includes: [u => u.OperationClaim, u => u.User]);

               var isAdmin =  receiverOperationClaims.Results.Any(u => u.OperationClaim.Name == Admin);

                if (!isAdmin)
                    throw new BusinessException("Yalnızca yöneticiye mesaj gönderebilirsiniz");
            }


            QueueFactory.SendMessageToExchange(exchangeName: "MessageExchange",
                                                exchangeType: "direct",
                                                queueName: "SendMessageQueue",
                                                obj:new SendMessageEvent
                                                {
                                                    SenderId = senderId,
                                                    Message = request.Text,
                                                    SendedTime = DateTime.Now,
                                                    ReceiverId = request.ReceiverId
                                                    
                                                });


            return new SendMessageResponse()
            {
                SendingTime = DateTime.Now,
                Text = request.Text
            };

        }
    }
}
