using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Residents.Login;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Security.JWT;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using System.Linq.Expressions;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Commands.Login
{
    public class ResidentLoginTests : ResidentMockRepository
    {
        private readonly ResidentLoginCommand _command;
        private readonly ResidentLoginCommandHandler _handler;
        private readonly ResidentLoginCommandValidator _validator;
        private readonly Mock<ITokenHelper> _tokenHelper;
        private readonly Mock<IUserOperationClaimRepository> _userOperationClaimRepository;
        public ResidentLoginTests(ResidentFakeDatas fakeData, ResidentLoginCommand command, ResidentLoginCommandValidator validator) : base(fakeData)
        {
            var list = new PagedViewModel<UserOperationClaim>()
                {
                  Results = {  new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        OperationClaimId = Guid.NewGuid(),
                        OperationClaim = new()
                        {
                            Id = Guid.NewGuid(),
                           Name = "Test",
                        }
                    }
                }
                };
            _tokenHelper = new Mock<ITokenHelper>();
            _userOperationClaimRepository = new Mock<IUserOperationClaimRepository>();
            _userOperationClaimRepository.Setup(x => x.GetListAsync(
                       It.IsAny<Expression<Func<UserOperationClaim, bool>>>(),
                       It.IsAny<Func<IQueryable<UserOperationClaim>, IOrderedQueryable<UserOperationClaim>>>(),
                       It.IsAny<int>(),
                       It.IsAny<int>(),
                       It.IsAny<bool>(),
                       It.IsAny<CancellationToken>(),
                       It.IsAny<Expression<Func<UserOperationClaim, object>>[]>())).ReturnsAsync(list);
            _command = command;
            _validator = validator;
            _handler = new(BusinessRules, _tokenHelper.Object, _userOperationClaimRepository.Object);
        }


        [Fact]
        public void EmailAndIdenticalNumberEmpty_ShouldReturn_ValidationError()
        {
            //Arrange
            _command.Email = string.Empty;
            _command.IdenticalNumber = string.Empty;
            //Act
            ValidationFailure? response =  _validator.Validate(_command).Errors.FirstOrDefault();
            //Assert
            Assert.NotEmpty(response.ErrorMessage);
            Assert.Equal(ResidentMessages.ValidationMessages.EmailOrIdenticalNumberCannotBeEmpty, response.ErrorMessage);
        }
        [Fact]
        public void InvalidEmail_ShouldReturn_ValidationError()
        {
            //Arrange
            _command.Email = "mehmet";
            _command.IdenticalNumber = string.Empty;
            //Act
            ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();
            //Assert
            Assert.NotEmpty(response.ErrorMessage);
            Assert.Equal(ResidentMessages.ValidationMessages.InvalidEmail, response.ErrorMessage);
        }
        [Fact]
        public void InvalidIdenticalNumber_ShouldReturn_EmptyValidationError()
        {
            //Arrange
            _command.Email = string.Empty;
            _command.IdenticalNumber = "12345";
            //Act
            ValidationResult result = _validator.Validate(_command);
            //Assert
            ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();
            //Assert
            Assert.NotEmpty(response.ErrorMessage);
            Assert.Equal(ResidentMessages.ValidationMessages.IdenticalNumberMustIncludeElevenChar, response.ErrorMessage);
        }
        [Fact]
        public void ValidEmail_ShouldReturn_EmptyValidationError()
        {
            //Arrange
            _command.Email = "mehmet@gmail.com";
            _command.IdenticalNumber = string.Empty;
            //Act
            ValidationResult result = _validator.Validate(_command);
            //Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
        [Fact]
        public void ValidIdenticalNumber_ShouldReturn_EmptyValidationError()
        {
            //Arrange
            _command.Email = string.Empty;
            _command.IdenticalNumber = "12345678910";
            //Act
            ValidationResult result = _validator.Validate(_command);
            //Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task EmaiDoesNotExistInDb_ShouldReturn_BusinessException()
        {
            //Arrange
            _command.Email = ResidentFakeDatas.NotInDbEmail;
            _command.Password = "12345";
            //Act
            async Task Action() => await _handler.Handle(_command, CancellationToken.None);
            //Assert
            var response = await Assert.ThrowsAsync<BusinessException>(Action);
            Assert.Equal(ResidentMessages.RuleMessages.ResidentCannotBeFound, response.Message);
        }
        [Fact]
        public async Task ValidLogin_ShouldReturn_NotNullResponse()
        {
            //Arrange
            _command.Email = ResidentFakeDatas.InDbEmail;
            _command.Password = "12345";
            //Act
            var response = await _handler.Handle(_command, CancellationToken.None);
            //Assert
           
            Assert.NotNull(response);
        }

    }
}
