using Moq;
using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Helpers;
using SiteManagement.XUnitTests.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Mock.Repositories.Commons;

namespace SiteManagement.XUnitTests.Mock.Repositories.Buildings;

public class BlockMockRepository : BaseMockRepository<IBlockRepository, Block, SiteManagementMapingProfile, BlockBusinessRules, BlockFakeDatas>
{
    public BlockMockRepository(BlockFakeDatas fakeData) : base(fakeData)
    {

        MockRepository
            .Setup(s =>
                s.IsBlockNameUnique(
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(
                (
                    string name
                ) =>
                {

                    return !fakeData.Data.Any(x => x.Name == name);
                    
                   

                }
            );

    }

}
