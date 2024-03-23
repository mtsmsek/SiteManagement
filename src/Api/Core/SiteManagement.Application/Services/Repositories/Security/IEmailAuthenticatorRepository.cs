using SiteManagement.Application.Services.Repositories.Commons;
using SiteManagement.Domain.Entities.Security;

namespace SiteManagement.Application.Services.Repositories.Security;

public interface IEmailAuthenticatorRepository: IAsyncRepository<EmailAuthenticator>
{
}
