using FluentValidation.Results;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;
using System.ComponentModel.DataAnnotations;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Apartments.Commands.CreateApartment;

public class CreateApartmentTests : ApartmentMockRepository
{
    private readonly CreateApartmentCommand _command;
    private readonly CreateApartmentCommandValidator _validator;
    private readonly CreateApartmentCommandHandler _handler;
    public CreateApartmentTests(ApartmentFakeDatas fakeData, CreateApartmentCommand command, CreateApartmentCommandValidator validator) : base(fakeData)
    {
        var mockBlockBusinessRules = MockBlockBusinessRules.GetBlockBusinessRules();
        _command = command;
        _validator = validator;
        _handler = new(MockRepository.Object, Mapper, BusinessRules, mockBlockBusinessRules);
    }

    [Fact]
    public void MinusApartmentNumber_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.ApartmentNumber = -1;
        //Act
        ValidationFailure? error = _validator.Validate(_command)
            .Errors.FirstOrDefault();
        //Assert
        Assert.Equal(ApartmentMessages.ValidationMessages.ApartmentNumberCannotBeLowerThanOne, error?.ErrorMessage);

    }
    [Fact]
    public async Task TheBlockContainingApartmentDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.BlockId = BlockFakeDatas.NotInDbId;
        //Act
        Task Action() => _handler.Handle(_command,CancellationToken.None);
        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, exception?.Message);

    }

    //todo -- continue with other apartment tests

}
