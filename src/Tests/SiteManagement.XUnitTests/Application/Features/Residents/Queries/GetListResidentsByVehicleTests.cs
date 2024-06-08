using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentsByVehicle;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using SiteManagement.XUnitTests.Application.Mock.Rules.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Queries;

public class GetListResidentsByVehicleTests : ResidentMockRepository
{
    private readonly GetListResidentsByVehicleQuery _query;
    private readonly GetListResidentsByVehicleQueryHandler _handler;
    public GetListResidentsByVehicleTests(ResidentFakeDatas fakeData, GetListResidentsByVehicleQuery query) : base(fakeData)
    {
        var vehicleBusinessRules = MockVehicleBusinessRules.GetVehicleBusinessRules();
        var residentVehicleRepository = new Mock<IResidentVehicleRepository>();
        _query = query;
        _handler = new(MockRepository.Object, Mapper, vehicleBusinessRules, residentVehicleRepository.Object);
    }
    
    [Fact]
    public async Task VehicleDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.VehicleRegistrationPlate = VehicleFakeData.NotInDbRegistrationPlate;
        //Act
        Task Action() => _handler.Handle(_query, CancellationToken.None);

        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(VehicleMessages.RuleMessages.NoVehicleFoundBelongToIndicatedRegistrationPlate, response.Message);
    }
    [Fact]
    public async Task InDbNameVehicleRegistrationPlate_ShouldReturn_OneResident()
    {
        //Arrange
        _query.VehicleRegistrationPlate = VehicleFakeData.InDbRegistraionPlate;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);

        //Assert
        Assert.Equal(1, response.Results.Count);
    }
}
