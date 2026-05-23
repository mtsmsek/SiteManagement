using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Residency.Commands.AddVehicle;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Tests.Residency;

/// <summary>
/// Unit tests for <see cref="AddVehicleCommandHandler"/>: load the resident,
/// register the vehicle, and save — or 404 when the resident is unknown.
/// </summary>
public class AddVehicleCommandHandlerTests
{
    private readonly IResidentRepository _residentRepository = Substitute.For<IResidentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private static Resident SampleResident()
        => Resident.Create(
            TcNo.From("10000000146"),
            FullName.Create("Ada", "Lovelace"),
            Email.From("ada@example.com"),
            PhoneNumber.From("05321234567"));

    [Fact]
    public async Task Handle_AddsTheVehicleAndSaves()
    {
        // arrange
        var resident = SampleResident();
        _residentRepository.GetByIdAsync(resident.Id, Arg.Any<CancellationToken>()).Returns(resident);
        var sut = CreateHandler();

        // act
        await sut.Handle(new AddVehicleCommand(resident.Id, "34ABC123", null), CancellationToken.None);

        // assert
        resident.Vehicles.Should().ContainSingle()
            .Which.Plate.Value.Should().Be("34ABC123");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownResident_Throws()
    {
        // arrange
        _residentRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Resident?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new AddVehicleCommand(Guid.NewGuid(), "34ABC123", null), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private AddVehicleCommandHandler CreateHandler() => new(_residentRepository, _unitOfWork);
}
