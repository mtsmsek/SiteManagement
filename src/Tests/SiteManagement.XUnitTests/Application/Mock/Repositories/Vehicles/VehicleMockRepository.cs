using Moq;
using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Commons;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles
{
    public class VehicleMockRepository : BaseMockRepository<IVehicleRepository, Vehicle, SiteManagementMapingProfile, VehicleBusinessRules, VehicleFakeData>
    {
        public VehicleMockRepository(VehicleFakeData fakeData) : base(fakeData)
        {
            BusinessRules = SetBusinessRules();
        }

        public override VehicleBusinessRules SetBusinessRules()
        {
            return new VehicleBusinessRules(MockRepository.Object, new Mock<ResidentMockRepository>(new ResidentFakeDatas()).Object.MockRepository.Object);
        }
    }
}
