using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.OpenDuesPeriod;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="OpenDuesPeriodCommandHandler"/>: build a new dues
/// period from the request, register it with the repository, and save.
/// </summary>
public class OpenDuesPeriodCommandHandlerTests
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = Substitute.For<IDuesPeriodRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_OpensThePeriodAddsItAndSaves()
    {
        // arrange
        var sut = CreateHandler();
        var cmd = new OpenDuesPeriodCommand(Guid.NewGuid(), 2026, 1, 500m);

        // act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // assert
        result.DuesPeriodId.Should().NotBeEmpty();
        await _duesPeriodRepository.Received(1).AddAsync(Arg.Any<DuesPeriod>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private OpenDuesPeriodCommandHandler CreateHandler() => new(_duesPeriodRepository, _unitOfWork);
}
