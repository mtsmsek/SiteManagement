using Moq;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

namespace SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;

public static class MockApartmentBusinessRules
{
    public static ApartmentBusinessRules GetApartmentBusinessRules()
    {
        var apartmentRepository = new ApartmentMockRepository(new ApartmentFakeDatas());
        var blockRepository = new BlockMockRepository(new BlockFakeDatas());

        var mockBusinessRule = new Mock<ApartmentBusinessRules>(apartmentRepository.MockRepository.Object, blockRepository.MockRepository.Object );
        return mockBusinessRule.Object;

    }
}
