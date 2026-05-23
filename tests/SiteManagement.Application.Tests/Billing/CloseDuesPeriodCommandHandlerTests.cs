using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.CloseDuesPeriod;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="CloseDuesPeriodCommandHandler"/>: load the period,
/// close it, and save — or 404 when the period is unknown.
/// </summary>
public class CloseDuesPeriodCommandHandlerTests
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = Substitute.For<IDuesPeriodRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ClosesThePeriodAndSaves()
    {
        // arrange
        var period = DuesPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), Money.Of(500m));
        _duesPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(new CloseDuesPeriodCommand(period.Id), CancellationToken.None);

        // assert
        period.IsClosed.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _duesPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((DuesPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new CloseDuesPeriodCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private CloseDuesPeriodCommandHandler CreateHandler() => new(_duesPeriodRepository, _unitOfWork);
}
