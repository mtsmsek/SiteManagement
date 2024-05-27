using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Commands.UpdateResident;

public class UpdateResidentPasswordTests : ResidentMockRepository
{
    private readonly UpdateResidentPasswordCommand _command;
    private readonly UpdateResidentPasswordCommandHandler _handler;
    private readonly UpdateResidentPasswordCommandValidator _validator;
    public UpdateResidentPasswordTests(ResidentFakeDatas fakeData, UpdateResidentPasswordCommand command, UpdateResidentPasswordCommandValidator validator) : base(fakeData)
    {
        _command = command;
        _validator = validator;
        _handler = new(MockRepository.Object, BusinessRules);
    }

    [Fact]
    public void EmptyPassword_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.NewPassword = string.Empty;
        //Act
        ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();
        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.PasswordCannotBeEmpty, response.ErrorMessage);
    }

    [Fact]
    public void PasswordLengthLessThanEight_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.NewPassword = "1234567";

        //Act
        ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();
        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.PasswordShouldLongerThanEightCharAndLessThanOrEqualToSixTeenChar,
                     response.ErrorMessage);
    }

    [Fact]
    public void PasswordLengthLongerThanSixteen_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.NewPassword = "12345678910111213141516";

        //Act
        ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();
        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.PasswordShouldLongerThanEightCharAndLessThanOrEqualToSixTeenChar,
                     response.ErrorMessage);
    }

    [Fact]
    public void PasswordDoesNotIncludeCapitalLetter_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.NewPassword = "abcde12345.";
        //Act
        ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.PasswordShouldIncludeAtLeastOneBiggerOneNumberAndSpecialChar,
                      response.ErrorMessage);
    }
    [Fact]
    public void PasswordDoesNotIncludeNumber_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.NewPassword = "abcdefgh.";
        //Act
        ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.PasswordShouldIncludeAtLeastOneBiggerOneNumberAndSpecialChar,
                      response.ErrorMessage);
    }
    [Fact]
    public void PasswordDoesNotIncludeSpecialChar_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.NewPassword = "abcdefgh123";
        //Act
        ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.PasswordShouldIncludeAtLeastOneBiggerOneNumberAndSpecialChar,
                      response.ErrorMessage);
    }



    [Fact]
    public void ValidPassword_ShouldReturn_EmptyValidationFailure()
    {
        //Arrange
        _command.NewPassword = "Abcd1234.";
        //Act
        ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();

        //Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task ResidentDoesNotExistInDb_ShouldReturn_BusinessExcepiton()
    {
        //Arrange
        _command.Id = ResidentFakeDatas.NotInDbId;
        //Act
        Task Action() => _handler.Handle(_command, CancellationToken.None);
        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentMessages.RuleMessages.ResidentCannotBeFound, response.Message);
    }

    [Fact]
    public async Task WrongOldPassword_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.Id = ResidentFakeDatas.InDbId;
        _command.OldPassword = "1234"; //Actual Password is 12345
        _command.NewPassword = "A1234567.";
        //Act
        Task Action() => _handler.Handle(_command, CancellationToken.None);

        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentMessages.RuleMessages.OldPasswordWrong, response.Message);
    }
    [Fact]
    public async Task UpdatePasswordSuccessfully_ShouldCalled_UpdateAsyncOnce()
    {
        //Arrange
        _command.Id = ResidentFakeDatas.InDbId;
        _command.OldPassword = "12345"; //Actual Password is 12345
        _command.NewPassword = "A1234567.";
        //Act
        await _handler.Handle(_command, CancellationToken.None);

        //Assert

        MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Resident>(), It.IsAny<CancellationToken>()),Times.Once());
    }




}
