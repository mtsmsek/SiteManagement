using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tenancy.Commands.AssignResident;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Application.Tests.Tenancy;

/// <summary>
/// Unit tests for <see cref="AssignResidentCommandHandler"/>: both referenced
/// aggregates must exist before the assignment is created, added, and saved.
/// </summary>
public class AssignResidentCommandHandlerTests
{
    private readonly IApartmentAssignmentRepository _assignmentRepository = Substitute.For<IApartmentAssignmentRepository>();
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IResidentRepository _residentRepository = Substitute.For<IResidentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private static Resident SampleResident()
        => Resident.Create(
            TcNo.From("10000000146"),
            FullName.Create("Ada", "Lovelace"),
            Email.From("ada@example.com"),
            PhoneNumber.From("05321234567"));

    private static AssignResidentCommand SampleCommand()
        => new(Guid.NewGuid(), Guid.NewGuid(), TenantType.Owner, new DateOnly(2026, 1, 1));

    [Fact]
    public async Task Handle_CreatesTheAssignmentAddsItAndSaves()
    {
        // arrange — both apartment-owning site and resident resolve
        _siteRepository.FindContainingApartmentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Site.Create("Lavender Heights", "Address"));
        _residentRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(SampleResident());
        var sut = CreateHandler();

        // act
        var result = await sut.Handle(SampleCommand(), CancellationToken.None);

        // assert
        result.AssignmentId.Should().NotBeEmpty();
        await _assignmentRepository.Received(1).AddAsync(Arg.Any<ApartmentAssignment>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownApartment_Throws()
    {
        // arrange
        _siteRepository.FindContainingApartmentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Site?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(SampleCommand(), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_UnknownResident_Throws()
    {
        // arrange — apartment resolves but the resident is missing
        _siteRepository.FindContainingApartmentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Site.Create("Lavender Heights", "Address"));
        _residentRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Resident?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(SampleCommand(), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private AssignResidentCommandHandler CreateHandler()
        => new(_assignmentRepository, _siteRepository, _residentRepository, _unitOfWork);
}
