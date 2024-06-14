using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Vehicles.DeleteCehicle.HardDelete;
using SiteManagement.Application.Features.Commands.Vehicles.DeleteVehicle.HardDelete;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.Vehicles.Commands.DeleteVehicle;

public class HardDeleteVehicleTests : VehicleMockRepository
{
    private readonly HardDeleteVehicleCommand _command;
    private readonly HardDeleteVehicleCommandHandler _handler;

    public HardDeleteVehicleTests(VehicleFakeData fakeData, HardDeleteVehicleCommand command) : base(fakeData)
    {
        _command = command;
        _handler = new(MockRepository.Object);
    }

    [Fact]
    public async Task VehicleDoesNotExistIn_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.VehicleId = VehicleFakeData.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(VehicleMessages.RuleMessages.VehicleCannotFound, response.Message);
    }
    [Fact]
    public async Task VehicleExistsInDb_ShouldCalled_DeleteAysncOnce()
    {
        //Arrange
        _command.VehicleId = VehicleFakeData.InDbId;
        //Act
        var response = await _handler.Handle(_command, CancellationToken.None);
        //Assert
        Assert.Equal(1, response);
        MockRepository.Verify(x => x.DeleteAsync(It.IsAny<Vehicle>(), true, It.IsAny<CancellationToken>()), Times.Once());
    }
}
