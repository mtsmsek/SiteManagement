using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Application.Services.Repositories.Commons;

namespace SiteManagement.Application.Services.Repositories.Vehicles;

public interface IResidentVehicleRepository : IAsyncRepository<ResidentVehicle>
{
}
