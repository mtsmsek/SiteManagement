using SiteManagement.Application.Features.Queries.Vehicles.GetListVehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Vehicles.Queries;

public class GetListVehiclesTests : VehicleMockRepository
{
    private readonly GetListAllVehiclesQuery _query;
    private readonly GetListAllVehiclesQueryHandler _handler;
    public GetListVehiclesTests(VehicleFakeData fakeData, GetListAllVehiclesQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper);
    }
    [Fact]
    public async Task GetListMethod_ShouldReturn_OneVehicle()
    {
        //Arrange
        
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.NotEmpty(response.Results);
        Assert.Equal(1, response.Results.Count);
    }
}
