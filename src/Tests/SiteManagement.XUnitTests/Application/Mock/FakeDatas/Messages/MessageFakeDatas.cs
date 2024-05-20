using SiteManagement.Domain.Entities.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Messages;

public class MessageFakeDatas : BaseFakeData<Message>
{
    public override List<Message> CreateFakeData()
    {
        var datas = new List<Message>()
        {
            new()
            {
                //TODO - change user ids after creating user datas
                Id = InDbId,
                CreatedDate = DateTime.Now,
                Text = "Hello",
                ReceiverId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                IsSeen = true,
                
            },
            new()
            {
                //TODO - change user ids after creating user datas
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Text = "Hi",
                ReceiverId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                IsSeen = true,

            },
            new()
            {
                //TODO - change user ids after creating user datas
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Text = "How are you?",
                ReceiverId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                IsSeen = false,

            },
            new()
            {
                //TODO - change user ids after creating user datas
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Text = "Fine you?",
                ReceiverId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                IsSeen = false,

            },
            new()
            {
                //TODO - change user ids after creating user datas
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Text = "Fine",
                ReceiverId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                IsSeen = false,

            },
        };
        return datas;
    }
}
