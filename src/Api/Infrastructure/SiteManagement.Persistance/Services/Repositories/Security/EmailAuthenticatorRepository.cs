using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;

namespace SiteManagement.Persistance.Services.Repositories.Security;

public class EmailAuthenticatorRepository : EfAsyncRepository<EmailAuthenticator, SiteManagementApplicationContext>, IEmailAuthenticatorRepository
{
    public EmailAuthenticatorRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
    {
    }
}
