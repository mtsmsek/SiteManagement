using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Residency.Commands.RemoveVehicle;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Tests.Residency;

/// <summary>
/// Unit tests for <see cref="RemoveVehicleCommandHandler"/>: load the resident,
/// unregister the vehicle, and save — or 404 when the resident is unknown.
/// </summary>
public class RemoveVehicleCommandHandlerTests
{
    private readonly IResidentRepository _residentRepository = Substitute.For<IResidentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private static Resident SampleResidentWithVehicle()
    {
        var resident = Resident.Create(
            TcNo.From("10000000146"),
            FullName.Create("Ada", "Lovelace"),
            Email.From("ada@example.com"),
            PhoneNumber.From("05321234567"));
        resident.AddVehicle(VehicleInfo.Create(PlateNumber.From("34ABC123"), null));
        return resident;
    }

    [Fact]
    public async Task Handle_RemovesTheVehicleAndSaves()
    {
        // arrange
        var resident = SampleResidentWithVehicle();
        _residentRepository.GetByIdAsync(resident.Id, Arg.Any<CancellationToken>()).Returns(resident);
        var sut = CreateHandler();

        // act
        await sut.Handle(new RemoveVehicleCommand(resident.Id, "34ABC123"), CancellationToken.None);

        // assert
        resident.Vehicles.Should().BeEmpty();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownResident_Throws()
    {
        // arrange
        _residentRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Resident?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new RemoveVehicleCommand(Guid.NewGuid(), "34ABC123"), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private RemoveVehicleCommandHandler CreateHandler() => new(_residentRepository, _unitOfWork);
}
