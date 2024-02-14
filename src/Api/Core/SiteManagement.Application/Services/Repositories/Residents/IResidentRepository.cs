using SiteManagement.Domain.Entities.Residents;
using SiteManagemnt.Application.Services.Repositories.Commons;

namespace SiteManagemnt.Application.Services.Repositories.Residents;

public interface IResidentRepository : IAsyncRepository<Resident>
{
}
