using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.DeleteBlock.HardDelete;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Blocks.Commands.DeleteBlock.HardDelete
{
    public class HardDeleteBlockTests : BlockMockRepository
    {
        private readonly HardDeleteBlockCommand _command;
        private readonly HardDeleteBlockCommandHandler _handler;
        private readonly Guid Id;
       

        public HardDeleteBlockTests(BlockFakeDatas fakeData, HardDeleteBlockCommand command) : base(fakeData)
        {
            _command = command;
            _handler = new HardDeleteBlockCommandHandler(MockRepository.Object, BusinessRules);
            Id = fakeData.Data.FirstOrDefault()!.Id;
        }
        [Fact]
        public async Task HardDeleteSuccessfully_Should_CalledDeleteAsyncOnce()
        {
            //Arrange
            _command.Id = BlockFakeDatas.InDbId;

            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            MockRepository.Verify(s => s.DeleteAsync(It.IsAny<Block>(), It.IsAny<bool>(), CancellationToken.None),Times.Once());
        }

        [Fact]
        public async Task HardDelete_ShouldReturnError_WhenBlockDoesNotExistInDatabase()
        {
            //Arrange
            _command.Id = BlockFakeDatas.NotInDbId;
          
            //Act
            Task Action() => _handler.Handle(_command, CancellationToken.None);

            //Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(Action);
            MockRepository.Verify(s => s.DeleteAsync(It.IsAny<Block>(), It.IsAny<bool>(), CancellationToken.None), Times.Never());
            Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, exception.Message);
            
        }
    }
}
