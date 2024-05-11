using Moq;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Helpers;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

namespace SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;

public static class MockBlockBusinessRules
{

    public static BlockBusinessRules GetBlockBusinessRules()
    {
        var blockRepository = new BlockMockRepository(new BlockFakeDatas());    

        var mockBusinessRule = new Mock<BlockBusinessRules>(blockRepository.MockRepository.Object);
        return mockBusinessRule.Object;

    }
}
