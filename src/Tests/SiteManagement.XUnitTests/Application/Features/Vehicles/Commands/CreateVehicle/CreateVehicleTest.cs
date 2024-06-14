using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Vehicles.Commands.CreateVehicle;

public class CreateVehicleTest : VehicleMockRepository
{
    private readonly CreateVehicleCommand _command;
    private readonly CreateVehicleCommandHandler _handler;
    private readonly CreateVehicleCommandValidator _validator;
    private readonly Mock<IResidentVehicleRepository> _residentVehicleRepository;
    public CreateVehicleTest(VehicleFakeData fakeData, CreateVehicleCommand command, CreateVehicleCommandValidator validator) : base(fakeData)
    {
        var residentRepository = new Mock<ResidentMockRepository>(new ResidentFakeDatas());
        _residentVehicleRepository = new Mock<IResidentVehicleRepository>();
        var apartmentBusinessRules = MockApartmentBusinessRules.GetApartmentBusinessRules();
        _command = command;
        _validator = validator;
        _handler = new(MockRepository.Object, Mapper, residentRepository.Object.MockRepository.Object, BusinessRules, _residentVehicleRepository.Object, apartmentBusinessRules);
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
        Assert.Equal(VehicleMessages.ValidationMessages.RegistraionPlateCannotBeEmpty, response!.ErrorMessage);
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
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidRegistrationPlate, response!.ErrorMessage);

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
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidProvincePart, response!.ErrorMessage);

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
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidProvincePart, response!.ErrorMessage);

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
        Assert.Equal(VehicleMessages.ValidationMessages.VehicleTypeCannotBeEmpty, response!.ErrorMessage);

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
        Assert.Equal(VehicleMessages.ValidationMessages.InvalidVehicleType, response!.ErrorMessage);

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
    public async Task ApartmentDoesNotExistInDatabase_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.ApartmentId = ApartmentFakeDatas.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ApartmentMessages.RuleMessages.ApartmentCannotBeFound, response.Message);
    }

    [Fact]
    public async Task NoResidentInApartment_ShouldReturn_BsuinessExcepiton()
    {
        //Arrange
        _command.ApartmentId = ApartmentFakeDatas.EmptyApartmentId;
        _command.VehicleRegistrationPlate = "34 ABC 25";
        _command.VehicleType = 1;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);

        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(VehicleMessages.RuleMessages.ThereIsNoResidentLivingInApartment, response.Message);


    }
    [Fact]
    public async Task ValidData_ShouldCalled_AddAsyncAndAddRangeOnceMethodsOnce()
    {
        //Arrange
        _command.ApartmentId = ApartmentFakeDatas.InDbId;
        _command.VehicleRegistrationPlate = "34 ABC 25";
        _command.VehicleType = 1;
        //Act
        CreateVehicleResponse response = await _handler.Handle(_command, CancellationToken.None);

        //Assert
        MockRepository.Verify(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once());
        _residentVehicleRepository.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<ResidentVehicle>>(), It.IsAny<CancellationToken>()), Times.Once());


    }







}
