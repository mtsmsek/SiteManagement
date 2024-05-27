using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Commands.UpdateResident
{
    public class UpdateEmailTests : ResidentMockRepository
    {
        private readonly UpdateResidentEmailCommand _command;
        private readonly UpdateResidentEmailCommandHandler _handler;
        private readonly UpdateResidentEmailCommandValidator _validator;
        public UpdateEmailTests(ResidentFakeDatas fakeData, UpdateResidentEmailCommand command, UpdateResidentEmailCommandValidator validator) : base(fakeData)
        {
            _command = command;
            _validator = validator;
            _handler = new(MockRepository.Object, BusinessRules);
        }

        [Fact]
        public void NullEmail_ShouldReturn_ValidationException()
        {
            //Arrange
            _command.Email = null;

            //Act
            ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();

            //Assert
            Assert.Equal(ResidentMessages.ValidationMessages.EmailCannotBeEmpty, response.ErrorMessage);
        }
        [Fact]
        public void EmptyEmail_ShouldReturn_ValidationException()
        {
            //Arrange
            _command.Email = string.Empty;

            //Act
            ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault();

            //Assert
            Assert.Equal(ResidentMessages.ValidationMessages.EmailCannotBeEmpty, response.ErrorMessage);
        }

        [Fact]
        public void InvalidEmail_ShouldReturn_ValidationException()
        {
            //Arrange
            _command.Email = "mehmet";
            //Act
            ValidationFailure? response = _validator.Validate(_command).Errors.FirstOrDefault() ;
            //Assert
            Assert.Equal(ResidentMessages.ValidationMessages.InvalidEmail, response.ErrorMessage);
        }

        [Fact]
        public async Task ResidentDoesNotExistInDb_ShouldReturn_BusinessException()
        {
            //Arrange
            _command.Id = ResidentFakeDatas.NotInDbId;
            //Act
            async Task Action() => await _handler.Handle(_command, CancellationToken.None);
            //Assert
            var response = await Assert.ThrowsAsync<BusinessException>(Action);
            Assert.Equal(ResidentMessages.RuleMessages.ResidentCannotBeFound, response.Message);
        }

        [Fact]
        public async Task EmailSameWithTheOldEmail_ShouldReturn_BusinessException()
        {
            //Arrange
            _command.Id = ResidentFakeDatas.InDbId;
            _command.Email = ResidentFakeDatas.InDbEmail;
            //Act
            async Task Action() => await _handler.Handle(_command, CancellationToken.None);
            //Assert
            var response = await Assert.ThrowsAsync<BusinessException>(Action);
            Assert.Equal(ResidentMessages.RuleMessages.NewEmailCannotBeSameWithTheOldEmail, response.Message);
        }

        [Fact]
        public async Task UpdateSuccessfully_ShouldReturn_SameEmailInResponseWithCommand()
        {
            //Arrange
            _command.Id = ResidentFakeDatas.InDbId;
            _command.Email = "abcd@test.com";
            //Act
            var response = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Resident>(),CancellationToken.None), Times.Once());
            Assert.Equal(_command.Email, response.Email);
        }



    }
}
