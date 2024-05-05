using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Blocks.Queries.GetListAllBlocks;

public class GetListAllBlocksTests : BlockMockRepository
{
    private readonly GetListAllBlockQuery _query;
    private readonly GetListAllBlockQueryHandler _handler;
    public GetListAllBlocksTests(BlockFakeDatas fakeData, GetListAllBlockQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper);
    }
    [Fact]
    public async void TotalBlockNumber_Should_BeSameWithTotalFakeDataCount()
    {
        //Arrange

        //Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        //Assert

        Assert.Equal(BlockFakeDatas.TotalDataCount, result.Results.Count);

    }
}
