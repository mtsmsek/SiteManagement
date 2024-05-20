using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Services.Security;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using SiteManagement.XUnitTests.Application.Mock.Services.Security;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Commands.CreateResident;

public class CreateResidentTests : ResidentMockRepository
{
    private readonly CreateResidentCommand _command;
    private readonly CreateResidentCommandHandler _handler;
    private readonly CreateResidentValidator _validator;
    private readonly Mock<IUserOperationClaimService> _userOperationClaimService;

    public CreateResidentTests(ResidentFakeDatas fakeData, CreateResidentCommand command, CreateResidentValidator validator) : base(fakeData)
    {
        //create class for mock
         _userOperationClaimService = MockIUserOpreationClaimService.GetIUserOperationClaimServiceInstance();
        _command = command;
        _handler = new(MockRepository.Object, BusinessRules, Mapper, _userOperationClaimService.Object);
        _validator = validator;
    }


    [Fact]
    public void FirstNameEmpty_ShouldReturn_ValidatonError()
    {
        //Arrange
        _command.FirstName = string.Empty;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();
            
        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.FirstNameCannotBeEmpty, response?.ErrorMessage);
    }
    [Fact]
    public void LastNameEmpty_ShouldReturn_ValidatonError()
    {
        //Arrange
        _command.FirstName = "Mehmet";
        _command.LastName = string.Empty;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.LastNameCannotBeEmpty, response?.ErrorMessage);
    }
    [Fact]
    public void BirthYearEmpty_ShouldReturn_ValidatonError()
    {
        //Arrange
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = default;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.BirthYearCannotBeEmpty, response?.ErrorMessage);
    }
    [Fact]
    public void BirthYearGreaterThanCurrentYear_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now;
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year + 1;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.InvalidBirthYear, response?.ErrorMessage);
    }
    [Fact]
    public void BirthMonthEmpty_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now;
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = default;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.BirthMonthCannotBeEmpty, response?.ErrorMessage);
    }
    [Fact]
    public void BirthMonthOutsideOfOneAndTwelve_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now;
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = 13;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.MonthValueMustBeBetweenOneAndTwelve, response?.ErrorMessage);
    }
    [Fact]
    public void MonthValueGreaterThanCurrentMonthValueWhenBirthYearEqualsToCurrentYear_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now;
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        if (currentTime.Month == 12)
            currentTime.AddMonths(-1);
        
        _command.BirthMonth = currentTime.Month + 1;
        _command.BirthDay = currentTime.Day;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.InvalidBirthMonth, response?.ErrorMessage);
    }
    [Fact]
    public void EmptyBirthDay_ShouldReturn_ValidatonError()
    {
        //Arrange

        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = 1997;
        _command.BirthMonth = 1;
        _command.BirthDay = default;

        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.BirthDayCannotBeEmpty, response?.ErrorMessage);
    }
    [Fact]
    public void BirthDayGreaterThanThirtyOne_ShouldReturn_ValidatonError()
    {
        //Arrange

        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = 1997;
        _command.BirthMonth = 2;
        _command.BirthDay = 30;

        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.InvalidBirthDay, response?.ErrorMessage);
    }

    [Fact]
    public void EmailEmpty_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = string.Empty;
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.EmailCannotBeEmpty, response?.ErrorMessage);
    }
    [Fact]
    public void InvalidEmail_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "memo";
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.InvalidEmail, response?.ErrorMessage);
    }
    [Fact]
    public void EmailWithoutExtension_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "memo@";
        _command.IdenticalNumber = "111";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.InvalidEmail, response?.ErrorMessage);
    }
    [Fact]
    public void IdenticalNumberLengthGreaterThanEleven_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "mehmet@gmail.com";
        _command.IdenticalNumber = "123456789101";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.IdenticalNumberMustIncludeElevenChar, response?.ErrorMessage);
    }
    [Fact]
    public void IdenticalNumberLengthLessThanEleven_ShouldReturn_ValidatonError()
    {
        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "mehmet@gmail.com";
        _command.IdenticalNumber = "123456789";
        //Act
        ValidationFailure? response = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(ResidentMessages.ValidationMessages.IdenticalNumberMustIncludeElevenChar, response?.ErrorMessage);
    }
    [Fact]
    public void ValidData_ShouldReturn_EmptyValidationFailure()
    {
        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "mehmet@gmail.com";
        _command.IdenticalNumber = "12345678910";
        _command.PhoneNumber = "12345";
        //Act
        ValidationResult result = _validator.Validate(_command);
           

        //Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);    
    }

    [Fact]
    public async Task DuplicatedIdenticalNumber_ShouldReturn_BusinessException()
    {
        
        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "mehmet@gmail.com";
        _command.PhoneNumber="12345";
        _command.IdenticalNumber = "10987654321";
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);

        //Assert
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentMessages.RuleMessages.ResidentAlreadyExists, response.Message);
    }
    [Fact]
    public async Task CreatedResidentSuccessfully_ShouldCalled_AddUserWithOperationClaimOnce()
    {

        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "mehmet@gmail.com";
        _command.PhoneNumber = "12345";
        _command.IdenticalNumber = "11987654321";
        //Act
        var response = await _handler.Handle(_command, CancellationToken.None);

        //Assert
        _userOperationClaimService.Verify(x => x.AddUserWithOperationClaim(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once());

       

    }
    [Fact]
    public async Task CreatedResidentSuccessfully_ShouldReturn_RandomPasswordInReponse()
    {

        //Arrange
        var currentTime = DateTime.Now.AddYears(-25);
        _command.FirstName = "Mehmet";
        _command.LastName = "Şimşek";
        _command.BirthYear = currentTime.Year;
        _command.BirthMonth = currentTime.Month;
        _command.BirthDay = currentTime.Day;
        _command.Email = "mehmet@gmail.com";
        _command.PhoneNumber = "12345";
        _command.IdenticalNumber = "11987654321";
        //Act
        var response = await _handler.Handle(_command, CancellationToken.None);

        //Assert
       
        Assert.NotEmpty(response.Password);
 
    }







}
