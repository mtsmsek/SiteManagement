using SiteManagement.Domain.Entities.Invoices;
using SiteManagemnt.Application.Services.Repositories.Commons;

namespace SiteManagemnt.Application.Services.Repositories.Invoices;

public interface IBillReposiotry : IAsyncRepository<Bill>
{
}
