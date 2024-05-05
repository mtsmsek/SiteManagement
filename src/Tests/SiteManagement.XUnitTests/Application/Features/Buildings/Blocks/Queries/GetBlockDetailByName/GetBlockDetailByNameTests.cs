using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Blocks.Queries.GetBlockDetailByName;

public class GetBlockDetailByNameTests : BlockMockRepository
{
    private readonly GetBlockDetailByNameQuery _query;
    private readonly GetBlockDetailByNameQueryHandler _handler;
    public GetBlockDetailByNameTests(BlockFakeDatas fakeData, GetBlockDetailByNameQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper, BusinessRules);
    }

    [Fact]
    public async Task BlockNameDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.Name = BlockFakeDatas.NotInDbBlockName;
        //Act
        Task Action() => _handler.Handle(_query, CancellationToken.None);
        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, exception?.Message);
    }
    [Fact]
    public async Task GetBlockDetailNameSuccessfully_Should_BeSameWithTheRequestName()
    {
        //Arrange
        _query.Name = BlockFakeDatas.InDbBlockName;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.Equal(BlockFakeDatas.InDbBlockName, response.Name);
        Assert.NotNull(response);
        
    }
}
