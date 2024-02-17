using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Commons;
using SiteManagement.Application.Services.Repositories.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Services.Repositories.Vehicles
{
    public class VehicleRepository : EfAsyncRepository<Vehicle, SiteManagementApplicationContext>,
        IVehicleRepository
    {
        public VehicleRepository(SiteManagementApplicationContext dbContext) : base(dbContext)
        {
        }
    }
}
