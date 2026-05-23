using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.CloseUtilityBillPeriod;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="CloseUtilityBillPeriodCommandHandler"/>: load the
/// period, close it, and save — or 404 when the period is unknown.
/// </summary>
public class CloseUtilityBillPeriodCommandHandlerTests
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = Substitute.For<IUtilityBillPeriodRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ClosesThePeriodAndSaves()
    {
        // arrange
        var period = UtilityBillPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), UtilityType.Electricity, Money.Of(300m));
        _utilityBillPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(new CloseUtilityBillPeriodCommand(period.Id), CancellationToken.None);

        // assert
        period.IsClosed.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _utilityBillPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((UtilityBillPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new CloseUtilityBillPeriodCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private CloseUtilityBillPeriodCommandHandler CreateHandler() => new(_utilityBillPeriodRepository, _unitOfWork);
}
