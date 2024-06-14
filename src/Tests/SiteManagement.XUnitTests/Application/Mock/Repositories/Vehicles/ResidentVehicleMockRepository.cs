using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.ResidentVehicles;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Commons;
using SiteManagement.XUnitTests.Application.Mock.Rules.Residents;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;

public class ResidentVehicleMockRepository : BaseMockRepository<IResidentVehicleRepository, ResidentVehicle, SiteManagementMapingProfile, ResidentVehicleBusinessRules, ResidentVehicleFakeDatas>
{
    public ResidentVehicleMockRepository(ResidentVehicleFakeDatas fakeData) : base(fakeData)
    {
        BusinessRules = SetBusinessRules();
    }

    public override ResidentVehicleBusinessRules SetBusinessRules()
    {
        return new ResidentVehicleBusinessRules(MockRepository.Object, MockResidentBusinessRules.GetResidentBusinessRules());
    }
}
