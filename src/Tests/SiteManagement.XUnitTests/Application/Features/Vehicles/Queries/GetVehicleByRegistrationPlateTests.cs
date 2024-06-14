using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Vehicles.GetVehicleByRegistrationPlate;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.Vehicles.Queries;

public class GetVehicleByRegistrationPlateTests : VehicleMockRepository
{
    private readonly GetVehicleByRegistrationPlateQuery _query;
    private readonly GetVehicleByRegistrationPlateQueryHandler _handler;
    public GetVehicleByRegistrationPlateTests(VehicleFakeData fakeData, GetVehicleByRegistrationPlateQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper);
    }
    [Fact]
    public async Task VehicleRegistrationPlateDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.VehicleRegistrationPlate = VehicleFakeData.NotInDbRegistrationPlate;
        //Act
        async Task Action() => await _handler.Handle(_query, CancellationToken.None);

        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(VehicleMessages.RuleMessages.NoVehicleFoundBelongToIndicatedRegistrationPlate,response.Message);
    }
    [Fact]
    public async Task VehicleRegistrationPlateExistsInDb_ShouldReturn_OneVehicle()
    {
        //Arrange
        _query.VehicleRegistrationPlate = VehicleFakeData.InDbRegistraionPlate;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);

        //Assert
        
        Assert.Equal(_query.VehicleRegistrationPlate, response.VehicleRegistrationPlate);
        Assert.NotNull( response.VehicleType);
    }

}
