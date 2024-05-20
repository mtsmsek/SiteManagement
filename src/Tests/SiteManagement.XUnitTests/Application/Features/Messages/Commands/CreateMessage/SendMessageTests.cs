using Microsoft.AspNetCore.Http;
using Moq;
using SiteManagement.Application.Features.Commands.Messages.SendMessage;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Messages;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Messages.Commands.CreateMessage
{
    public class SendMessageTests : MessageMockRepository
    {
        private readonly SendMessageCommand _command;
        private readonly SendMessageCommandHandler _handler;


        public SendMessageTests(MessageFakeDatas fakeData, SendMessageCommand command) : base(fakeData)
        {
            //TODO arrange these in external classes
            var httpAccessor = new Mock<IHttpContextAccessor>();
            var userOperationClaimRepository = new Mock<IUserOperationClaimRepository>();
            _command = command;
            _handler = new(MockRepository.Object, Mapper, httpAccessor.Object, userOperationClaimRepository.Object);
        }
        //TODO -- complete here later


    }
}
