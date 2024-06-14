using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.VehicleResident.DeleteVehicleResident.HardDelete;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.ResidentVehicles.Commands.DeleteResidentVehicle;

public class HardDeleteResidentVehicleTests : ResidentVehicleMockRepository
{
    private readonly HardDeleteResidentVehicleCommand _command;
    private readonly HardDeleteResidentVehicleCommandHandler _handler;
    public HardDeleteResidentVehicleTests(ResidentVehicleFakeDatas fakeData, HardDeleteResidentVehicleCommand command) : base(fakeData)
    {
        _command = command;
        _handler = new(MockRepository.Object, BusinessRules);
    }
    [Fact]
    public async Task ResidentVehicleDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.ResidentVehicleId = ResidentVehicleFakeDatas.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentVehicleMessages.RuleMessages.ResidentOrVehicleCannotBeFound, response.Message);

    }

    [Fact]
    public async Task ResidentVehicleExistsInDb_ShouldCalled_DeleteAsyncMethodOnce()
    {
        //Arrange
        _command.ResidentVehicleId = ResidentVehicleFakeDatas.InDbId;
        //Act
        var response =  await _handler.Handle(_command, CancellationToken.None);
        //Assert
        MockRepository.Verify(x => x.DeleteAsync(It.IsAny<ResidentVehicle>(), true, It.IsAny<CancellationToken>()),Times.Once());
        Assert.Equal(1, response);

    }
}
