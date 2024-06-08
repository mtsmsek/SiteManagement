using Moq;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;

namespace SiteManagement.XUnitTests.Application.Mock.Rules.Vehicles;

public static class MockVehicleBusinessRules
{

    public static VehicleBusinessRules GetVehicleBusinessRules()
    {
        var vehicleRepository = new VehicleMockRepository(new VehicleFakeData());
        var residentRepository = new ResidentMockRepository(new ResidentFakeDatas());

        var mockBusinessRule = new Mock<VehicleBusinessRules>(vehicleRepository.MockRepository.Object, residentRepository.MockRepository.Object);
        return mockBusinessRule.Object;
    }
}
