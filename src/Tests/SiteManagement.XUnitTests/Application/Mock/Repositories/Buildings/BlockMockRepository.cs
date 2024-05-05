using Moq;
using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

public class BlockMockRepository : BaseMockRepository<IBlockRepository, Block, SiteManagementMapingProfile, BlockBusinessRules, BlockFakeDatas>
{
    public BlockMockRepository(BlockFakeDatas fakeData) : base(fakeData)
    {
        BusinessRules = SetBusinessRules();

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
        
        MockRepository
            .Setup(s=> 
            s.IsBlockExist(It.IsAny<Guid>()))!
            .ReturnsAsync((Guid id) => 
            {
                var result = fakeData.Data.FirstOrDefault(x => x.Id == id);
                
                return result;
            });
        MockRepository
           .Setup(s =>
           s.IsBlockExist(It.IsAny<string>()))!
           .ReturnsAsync((string name) =>
           {
               var result = fakeData.Data.FirstOrDefault(x => x.Name== name);

               return result;
           });

    }

    public override BlockBusinessRules SetBusinessRules()
    {
        return new BlockBusinessRules(MockRepository.Object);
    }
}
