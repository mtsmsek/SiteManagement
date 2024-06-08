using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Residents.GetResidentByIdenticalNumber;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Queries;

public class GetResidentByIdenticalNumberTests : ResidentMockRepository
{
    private readonly GetResidentByIdenticalNumberQuery _query;
    private readonly GetResidentByIdenticalNumberQueryHandler _handler;
    public GetResidentByIdenticalNumberTests(ResidentFakeDatas fakeData, GetResidentByIdenticalNumberQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper);
       
    }
    [Fact]
    public async Task IdenticalNumberDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.IdenticalNumber = "12345";

        //Act
        async Task Action() => await _handler.Handle(_query, CancellationToken.None);

        //Assert
       var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentMessages.RuleMessages.ResidentWithIdenticalNumberDoesNotExist, response.Message);  
    }
    [Fact]
    public async Task IdenticalNumberExistInDb_ShouldReturn_OneResident()
    {
        //Arrange
        _query.IdenticalNumber = ResidentFakeDatas.InDbIdenticalNumber;

        //Act
        var response =  await _handler.Handle(_query, CancellationToken.None);

        //Assert
        Assert.NotNull(response.FirstName);
        Assert.NotNull(response.BlockName);
    }



}
