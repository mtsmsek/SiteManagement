using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByBlockName;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;


namespace SiteManagement.XUnitTests.Application.Features.Buildings.Apartments.Queries;

public class GetListApartmentByBlockNameTests : ApartmentMockRepository
{
    private readonly GetListApartmentsByBlockNameQuery _query;
    private readonly GetListApartmentsByBlockNameQueryHandler _handler;
    private readonly BlockBusinessRules _blockBusinessRules;
    public GetListApartmentByBlockNameTests(ApartmentFakeDatas fakeData, GetListApartmentsByBlockNameQuery query) : base(fakeData)
    {
        _blockBusinessRules = MockBlockBusinessRules.GetBlockBusinessRules();
        _query = query;
        _handler = new(MockRepository.Object, Mapper, _blockBusinessRules);
    }

    [Fact]
    public async Task WhenBlockNameIsNotEmptyButDoesNotExistInDatabase_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.BlockName = BlockFakeDatas.NotInDbBlockName;

        //Act
        async Task Action() => await _handler.Handle(_query, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, exception.Message);


    }
    [Fact]
    public async Task WhenBlockNameIsNotEmptyAndExistsInDatabase_ShouldReturn_ResponseDatas()
    {
        //Arrange
        _query.BlockName = BlockFakeDatas.InDbBlockName;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Results.Count);
        Assert.Equal("A", response.Results.FirstOrDefault()!.BlockName);


    }
}
