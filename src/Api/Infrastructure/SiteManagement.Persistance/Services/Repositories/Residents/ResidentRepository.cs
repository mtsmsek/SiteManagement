using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Persistance.Services.Repositories.Residents;

public class ResidentRepository : EfAsyncRepository<Resident, SiteManagementApplicationContext>,
    IResidentRepository
{
    public ResidentRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
    {
    }
}
