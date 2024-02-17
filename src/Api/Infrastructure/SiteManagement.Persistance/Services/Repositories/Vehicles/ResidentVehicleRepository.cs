using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagement.Application.Services.Repositories.Vehicles;

namespace SiteManagement.Persistance.Services.Repositories.Vehicles
{
    internal class ResidentVehicleRepository : EfAsyncRepository<ResidentVehicle,
                                                                  SiteManagementApplicationContext>,
                                                IResidentVehicleRepository
    {
        public ResidentVehicleRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }
    }
}
