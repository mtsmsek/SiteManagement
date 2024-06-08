using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentByBlockName;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Queries;

public class GetListResidentsByBlockNameTests : ResidentMockRepository
{
    private readonly GetListResidentsByBlockNameQuery _query;
    private readonly GetListResidentsByBlockNameQueryHandler _handler;

    public GetListResidentsByBlockNameTests(ResidentFakeDatas fakeData, GetListResidentsByBlockNameQuery query) : base(fakeData)
    {
        var blockBusinessRules = MockBlockBusinessRules.GetBlockBusinessRules();
        
        _query = query;
        _handler = new(MockRepository.Object, Mapper, blockBusinessRules);
    }
    [Fact]
    public async Task BlockDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Act
        _query.BlockName = BlockFakeDatas.NotInDbBlockName;

        //Action
        Task Action() => _handler.Handle(_query, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, response.Message);
    }
    [Fact]
    public async Task BlockExistsAndApartmentNumberTwo_ShouldReturn_TwoResident()
    {
        //Act
        _query.BlockName = BlockFakeDatas.InDbBlockName;
      
        //Action
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.Equal(2, response.Results.Count);
    }
    [Fact]
    public async Task BlockExistsAndApartmentNumberThree_ShouldReturn_TwoResident()
    {
        //Act
        _query.BlockName = BlockFakeDatas.InDbBlockName;

        //Action
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.Equal(2, response.Results.Count);
    }
}
