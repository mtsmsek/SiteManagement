using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.OpenUtilityBillPeriod;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="OpenUtilityBillPeriodCommandHandler"/>: build a new
/// utility bill period from the request, register it with the repository, and save.
/// </summary>
public class OpenUtilityBillPeriodCommandHandlerTests
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = Substitute.For<IUtilityBillPeriodRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_OpensThePeriodAddsItAndSaves()
    {
        // arrange
        var sut = CreateHandler();
        var cmd = new OpenUtilityBillPeriodCommand(Guid.NewGuid(), 2026, 1, UtilityType.Electricity, 300m);

        // act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // assert
        result.UtilityBillPeriodId.Should().NotBeEmpty();
        await _utilityBillPeriodRepository.Received(1).AddAsync(Arg.Any<UtilityBillPeriod>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private OpenUtilityBillPeriodCommandHandler CreateHandler() => new(_utilityBillPeriodRepository, _unitOfWork);
}
