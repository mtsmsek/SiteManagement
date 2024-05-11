using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Apartments.Commands.DeleteApartment.HardDeleteApartment;

public class HardDeleteApartmentTests : ApartmentMockRepository
{
    private readonly HardDeleteApartmentCommand _command;
    private readonly HardDeleteApartmentCommandHandler _handler;
    public HardDeleteApartmentTests(ApartmentFakeDatas fakeData, HardDeleteApartmentCommand command) : base(fakeData)
    {
        _command = command; 
        _handler = new HardDeleteApartmentCommandHandler(MockRepository.Object, BusinessRules);
    }

    [Fact]
    public async Task HardDeleteSuccessfully_Should_CalledDeleteAsyncOnce()
    {
        //Arrange
        _command.Id = ApartmentFakeDatas.InDbId;

        //Act
        await _handler.Handle(_command, CancellationToken.None);

        //Assert
        MockRepository.Verify(s => s.DeleteAsync(It.IsAny<Apartment>(), It.IsAny<bool>(), CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task HardDelete_ShouldReturnError_WhenApartmentDoesNotExistInDatabase()
    {
        //Arrange
        _command.Id = ApartmentFakeDatas.NotInDbId;

        //Act
        Task Action() => _handler.Handle(_command, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        MockRepository.Verify(s => s.DeleteAsync(It.IsAny<Apartment>(), It.IsAny<bool>(), CancellationToken.None), Times.Never());
        Assert.Equal(ApartmentMessages.RuleMessages.ApartmentCannotBeFound, exception?.Message);

    }

}
