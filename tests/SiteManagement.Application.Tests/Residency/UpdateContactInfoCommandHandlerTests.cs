using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Residency.Commands.UpdateContactInfo;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Tests.Residency;

/// <summary>
/// Unit tests for <see cref="UpdateContactInfoCommandHandler"/>: load the
/// resident, replace email + phone, and save — or 404 when the resident is unknown.
/// </summary>
public class UpdateContactInfoCommandHandlerTests
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
    public async Task Handle_UpdatesContactInfoAndSaves()
    {
        // arrange
        var resident = SampleResident();
        _residentRepository.GetByIdAsync(resident.Id, Arg.Any<CancellationToken>()).Returns(resident);
        var sut = CreateHandler();

        // act
        await sut.Handle(new UpdateContactInfoCommand(resident.Id, "grace@example.com", "05339876543"), CancellationToken.None);

        // assert
        resident.Email.Value.Should().Be("grace@example.com");
        resident.Phone.Should().Be(PhoneNumber.From("05339876543"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownResident_Throws()
    {
        // arrange
        _residentRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Resident?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new UpdateContactInfoCommand(Guid.NewGuid(), "grace@example.com", "05339876543"), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private UpdateContactInfoCommandHandler CreateHandler() => new(_residentRepository, _unitOfWork);
}
