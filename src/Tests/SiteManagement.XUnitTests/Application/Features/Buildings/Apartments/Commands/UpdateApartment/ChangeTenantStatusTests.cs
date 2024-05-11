using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Apartments.Commands.UpdateApartment;

public class ChangeTenantStatusTests : ApartmentMockRepository
{
    private readonly ChangeTenantStatusCommand _command;
    private readonly ChangeTenantStatusCommandHandler _handler;
    public ChangeTenantStatusTests(ApartmentFakeDatas fakeData, ChangeTenantStatusCommand command) : base(fakeData)
    {
        _command = command;
        _handler = new(MockRepository.Object,Mapper,BusinessRules);
    }
    [Fact]
    public async Task ChangeTenantStatusSuccessfully_Should_CalledUpdateAsyncOnce()
    {
        //Arrange
        _command.Id = ApartmentFakeDatas.InDbId;

        //Act
        await _handler.Handle(_command, CancellationToken.None);

        //Assert
        MockRepository.Verify(s => s.UpdateAsync(It.IsAny<Apartment>(), CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task ChangeTenantStatus_ShouldReturnError_WhenApartmentDoesNotExistInDatabase()
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
