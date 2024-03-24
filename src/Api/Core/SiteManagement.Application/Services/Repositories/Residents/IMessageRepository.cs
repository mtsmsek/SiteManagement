using SiteManagement.Application.Services.Repositories.Commons;
using SiteManagement.Domain.Entities.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Services.Repositories.Residents
{
    public interface IMessageRepository : IAsyncRepository<Message>
    {
    }
}
