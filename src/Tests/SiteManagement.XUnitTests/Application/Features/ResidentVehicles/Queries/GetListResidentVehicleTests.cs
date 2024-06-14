using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.ResidentVehicles.GetListResidentVehicles;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.ResidentVehicles.Queries;

public class GetListResidentVehicleTests : ResidentVehicleMockRepository
{
    private readonly GetListResidentVehiclesQuery _query;
    private readonly GetListResidentVehiclesQueryHandler _handler;
    public GetListResidentVehicleTests(ResidentVehicleFakeDatas fakeData, GetListResidentVehiclesQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper);
    }
    [Fact]
    public async Task ResidentDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.ResidentId = ResidentFakeDatas.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_query,CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentVehicleMessages.RuleMessages.ResidentOrVehicleCannotBeFound, response.Message);
    }
    [Fact]
    public async Task ResidentExistInDb_ShouldReturn_OneResidentVehicle()
    {
        //Arrange
        _query.ResidentId = ResidentFakeDatas.InDbId;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        
        Assert.Equal(1,response.Results.Count);
    }
}
