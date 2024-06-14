using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.VehicleResident.CreateVehicleResident;
using SiteManagement.Application.Rules.Vehicles;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Rules.Residents;
using SiteManagement.XUnitTests.Application.Mock.Rules.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.ResidentVehicles.Commands.CreateResidentVehicle;

public class CreateResidentVehicleTests : ResidentVehicleMockRepository
{
    private readonly CreateResidentVehicleCommand _command;
    private readonly CreateResidentVehicleCommandHandler _handler;


    public CreateResidentVehicleTests(ResidentVehicleFakeDatas fakeData, CreateResidentVehicleCommand command) : base(fakeData)
    {
        var residentBusinessRules = MockResidentBusinessRules.GetResidentBusinessRules();
        var vehicleBusinessRules = MockVehicleBusinessRules.GetVehicleBusinessRules();
        _command = command;
        _handler = new(MockRepository.Object, residentBusinessRules, vehicleBusinessRules, Mapper);
    }

    [Fact]
    public async Task ResidentDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.ResidentId = ResidentFakeDatas.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response =await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentMessages.RuleMessages.ResidentCannotBeFound, response.Message);
    }
    [Fact]
    public async Task VehicleDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.ResidentId = ResidentFakeDatas.InDbId;
        _command.VehicleId = VehicleFakeData.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(VehicleMessages.RuleMessages.VehicleCannotFound, response.Message);
    }
    [Fact]
    public async Task ValidData_ShouldCalled_AddAsyncMethodOnce()
    {
        //Arrange
        _command.ResidentId = ResidentFakeDatas.InDbId;
        _command.VehicleId = VehicleFakeData.InDbId;
        //Act
        var response = await _handler.Handle(_command, CancellationToken.None);
        //Assert
        MockRepository.Verify(x => x.AddAsync(It.IsAny<ResidentVehicle>(), It.IsAny<CancellationToken>()), Times.Once());
        Assert.Equal(1, response);
    }
}
