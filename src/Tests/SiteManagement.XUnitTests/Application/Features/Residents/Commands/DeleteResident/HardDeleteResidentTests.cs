using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Residents.DeleteResident.HardDelete;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Commands.DeleteResident
{
    public class HardDeleteResidentTests : ResidentMockRepository
    {
        private readonly HardDeleteResidentCommand _command;
        private readonly HardDeleteResidentCommandHandler _handler;
        public HardDeleteResidentTests(ResidentFakeDatas fakeData, HardDeleteResidentCommand command) : base(fakeData)
        {
            _command = command;
            _handler = new(MockRepository.Object, BusinessRules);
        }

        [Fact]
        public async Task ResidentDoesNotExistInDb_ShouldReturn_BusinessException()
        {
            //Arrange
            _command.Id = ResidentFakeDatas.NotInDbId;
            //Act
            async Task Action() => await _handler.Handle(_command,CancellationToken.None);
            //Assert
           var response = await Assert.ThrowsAsync<BusinessException>(Action);
            Assert.Equal(ResidentMessages.RuleMessages.ResidentCannotBeFound, response.Message);
        }

        [Fact]
        public async Task HardDeleteResidentSuccessfully_ShouldCalled_DeleteAsyncOnce()
        {
            //Arrange
            _command.Id = ResidentFakeDatas.InDbId;
            //Act
            var response = await _handler.Handle(_command, CancellationToken.None);
            //Assert
            MockRepository.Verify(x => x.DeleteAsync(It.IsAny<Resident>(), true, It.IsAny<CancellationToken>()),Times.Once());
            Assert.NotEqual(Guid.Empty, response);
        }


    }
}
