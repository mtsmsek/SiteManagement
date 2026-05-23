using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Email;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Residency.Commands.RegisterResident;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Application.Tests.Residency;

/// <summary>
/// Unit tests for <see cref="RegisterResidentCommandHandler"/>: create the
/// resident + linked Identity user and email the generated password, or reject a
/// duplicate citizenship number.
/// </summary>
public class RegisterResidentCommandHandlerTests
{
    private readonly IResidentRepository _residentRepository = Substitute.For<IResidentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserAuthService _userAuth = Substitute.For<IUserAuthService>();
    private readonly IPasswordGenerator _passwordGenerator = Substitute.For<IPasswordGenerator>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();

    private static RegisterResidentCommand SampleCommand()
        => new("10000000146", "Ada", "Lovelace", "ada@example.com", "05321234567");

    [Fact]
    public async Task Handle_CreatesResidentRegistersUserSendsWelcomeAndSaves()
    {
        // arrange
        _residentRepository.FindByTcNoAsync(Arg.Any<TcNo>(), Arg.Any<CancellationToken>()).Returns((Resident?)null);
        _passwordGenerator.Generate().Returns("Temp-P@ss1");
        var sut = CreateHandler();

        // act
        var result = await sut.Handle(SampleCommand(), CancellationToken.None);

        // assert
        result.ResidentId.Should().NotBeEmpty();
        await _residentRepository.Received(1).AddAsync(Arg.Any<Resident>(), Arg.Any<CancellationToken>());
        await _userAuth.Received(1).RegisterResidentUserAsync(
            Arg.Any<Guid>(), "ada@example.com", "Temp-P@ss1", Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _emailSender.Received(1).SendResidentWelcomeAsync(
            "ada@example.com", Arg.Any<string>(), "Temp-P@ss1", Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateTcNo_Throws()
    {
        // arrange
        var existing = Resident.Create(
            TcNo.From("10000000146"),
            FullName.Create("Ada", "Lovelace"),
            Email.From("ada@example.com"),
            PhoneNumber.From("05321234567"));
        _residentRepository.FindByTcNoAsync(Arg.Any<TcNo>(), Arg.Any<CancellationToken>()).Returns(existing);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(SampleCommand(), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }

    private RegisterResidentCommandHandler CreateHandler()
        => new(_residentRepository, _unitOfWork, _userAuth, _passwordGenerator, _emailSender);
}
