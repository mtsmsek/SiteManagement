using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;

namespace SiteManagement.XUnitTests.Application.Features.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleTests : VehicleMockRepository
{
    private readonly UpdateVehicleCommand _command;
    private readonly UpdateVehicleCommandHandler _handler;
    private readonly UpdateVehicleCommandValidator _validator;
    public UpdateVehicleTests(VehicleFakeData fakeData, UpdateVehicleCommand command, UpdateVehicleCommandValidator validator) : base(fakeData)
    {
        _command = command;
        _validator = validator;
        _handler = new(MockRepository.Object, Mapper, BusinessRules);
    }
    [Fact]
    public void EmptyRegistrationPlate_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.VehicleRegistrationPlate = string.Empty;
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();
        //Assert
        Assert.Equal(VehicleMessages.ValidationMessages.RegistraionPlateCannotBeEmpty, response.ErrorMessage);
    }

    [Fact]
    public void RegistrationPlateDoesNotConsistFrom3Parts_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.VehicleRegistrationPlate = "34 ABC"; //consist from 2 parts => actual data should like 34 ABC 4534
                                                      //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();
        //Assert
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidRegistrationPlate, response.ErrorMessage);

    }

    [Fact]
    public void RegistrationPlateProvincePartNotBetween1And81_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.VehicleRegistrationPlate = "83 ABC 3454";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidProvincePart, response.ErrorMessage);

    }
    [Fact]
    public void RegistrationPlateProvincePartNotIntegerValue_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.VehicleRegistrationPlate = "AA ABC 3454";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidProvincePart, response.ErrorMessage);

    }

    [Fact]
    public void EmptyVehicleType_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.VehicleRegistrationPlate = "31 ABC 245";
        _command.VehicleType = default;
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();
        //Assert
        Assert.Equal(VehicleMessages.ValidationMessages.VehicleTypeCannotBeEmpty, response.ErrorMessage);

    }
    [Fact]
    public void InvalidVehicleType_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.VehicleRegistrationPlate = "31 ABC 245";
        _command.VehicleType = VehicleType.Enumarations.Count + 1;
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();
        //Assert
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidVehicleType, response.ErrorMessage);

    }

    [Fact]
    public void ValidData_ShouldReturn_EmptyValidationFailure()
    {
        //Arrange
        _command.VehicleRegistrationPlate = "31 ABC 245";
        _command.VehicleType = VehicleType.Enumarations.FirstOrDefault().Key;
        //Act
        ValidationResult? response = _validator.Validate(_command);


        //Assert
        Assert.Empty(response.Errors);

    }

    [Fact]
    public async Task VehicleDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.Id = VehicleFakeData.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(VehicleMessages.RuleMessages.VehicleCannotFound, response.Message);
    }

    [Fact]
    public async Task ValidData_ShouldCalled_UpdateAsyncOnce()
    {
        //Arrange
        _command.Id = VehicleFakeData.InDbId;
        _command.VehicleRegistrationPlate = "31 ABC 245";
        _command.VehicleType = VehicleType.Enumarations.FirstOrDefault().Key;
        //Act
        UpdateVehicleCommandResponse response = await _handler.Handle(_command, CancellationToken.None);
        //Assert
        MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once());
        Assert.Equal(_command.VehicleRegistrationPlate, response.VehicleRegistrationPlate);


    }
}
