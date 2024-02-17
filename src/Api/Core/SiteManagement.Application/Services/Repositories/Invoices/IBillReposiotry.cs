using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Application.Services.Repositories.Commons;

namespace SiteManagement.Application.Services.Repositories.Invoices;

public interface IBillReposiotry : IAsyncRepository<Bill>
{
}
