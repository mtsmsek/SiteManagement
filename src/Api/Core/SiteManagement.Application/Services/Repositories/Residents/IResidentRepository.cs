using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Application.Services.Repositories.Commons;

namespace SiteManagement.Application.Services.Repositories.Residents;

public interface IResidentRepository : IAsyncRepository<Resident>
{
}
