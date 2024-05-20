using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Messages;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Messages
{
    public class MessageMockRepository : BaseMockRepository<IMessageRepository, Message, SiteManagementMapingProfile, MessageBusinessRules, MessageFakeDatas>
    {
        public MessageMockRepository(MessageFakeDatas fakeData) : base(fakeData)
        {
            BusinessRules = SetBusinessRules();
        }

        public override MessageBusinessRules SetBusinessRules()
        {
            return new MessageBusinessRules();
        }
    }
}
