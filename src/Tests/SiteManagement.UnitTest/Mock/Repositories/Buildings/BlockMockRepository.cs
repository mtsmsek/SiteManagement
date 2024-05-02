using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.UnitTest.Mock.FakeDatas.Buildings;
using SiteManagement.UnitTest.Mock.Repositories.Commons;

namespace SiteManagement.UnitTest.Mock.Repositories.Buildings;

public class BlockMockRepository : BaseMockRepository<IBlockRepository, Block, SiteManagementMapingProfile, BlockBusinessRules, BlockFakeDatas>
{
    public BlockMockRepository(BlockFakeDatas fakeData) : base(fakeData)
    {
    }
}
