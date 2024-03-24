using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;

namespace SiteManagement.Persistance.Services.Repositories.Residents
{
    public class MessageRepository : EfAsyncRepository<Message, SiteManagementApplicationContext>, IMessageRepository
    {
        public MessageRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }
    }
}
