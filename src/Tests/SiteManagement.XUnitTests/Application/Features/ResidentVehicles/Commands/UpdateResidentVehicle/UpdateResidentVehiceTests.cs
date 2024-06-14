using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.VehicleResident.UpdateVehicleResident;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Rules.Residents;
using SiteManagement.XUnitTests.Application.Mock.Rules.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.ResidentVehicles.Commands.UpdateResidentVehicle;

public class UpdateResidentVehiceTests : ResidentVehicleMockRepository
{
    private readonly UpdateResidentVehicleCommand _command;
    private readonly UpdateResidentVehicleCommandHandler _handler;
    public UpdateResidentVehiceTests(ResidentVehicleFakeDatas fakeData, UpdateResidentVehicleCommand command) : base(fakeData)
    {
        var residentRepository = new Mock<ResidentMockRepository>(new ResidentFakeDatas());
        var vehicleRepository = new Mock<VehicleMockRepository>(new VehicleFakeData());

        var residentBusinessRules = MockResidentBusinessRules.GetResidentBusinessRules();
        var vehicleBusinessRules = MockVehicleBusinessRules.GetVehicleBusinessRules();
     
        _command = command;
        _handler = new(MockRepository.Object, residentBusinessRules,BusinessRules, vehicleBusinessRules);
    }
    [Fact]
    public async Task ResidentVehicleDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.Id = ResidentVehicleFakeDatas.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentVehicleMessages.RuleMessages.ResidentOrVehicleCannotBeFound, response.Message);

    }
    [Fact]
    public async Task ResidentDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.Id = ResidentVehicleFakeDatas.InDbId;
        _command.UserId = VehicleFakeData.InDbId;
        _command.UserId = ResidentFakeDatas.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentMessages.RuleMessages.ResidentCannotBeFound, response.Message);

    }
    [Fact]
    public async Task VehiceDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.Id = ResidentVehicleFakeDatas.InDbId;
        _command.UserId = ResidentFakeDatas.InDbId;
        _command.VehicleId = VehicleFakeData.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(VehicleMessages.RuleMessages.VehicleCannotFound, response.Message);

    }
    [Fact]
    public async Task ValidData_ShouldCalled_UpdateAsyncMethodOnce()
    {
        //Arrange
        _command.Id = ResidentVehicleFakeDatas.InDbId;
        _command.UserId = ResidentFakeDatas.InDbId;
        _command.VehicleId = VehicleFakeData.InDbId;
        //Act
        var response = await _handler.Handle(_command, CancellationToken.None);
        //Assert
        MockRepository.Verify(x => x.UpdateAsync(It.IsAny<ResidentVehicle>(), It.IsAny<CancellationToken>()), Times.Once());
        Assert.True(response);
    }
}
