using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.MarkDuesItemPaid;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="MarkDuesItemPaidCommandHandler"/>: load the period,
/// mark the item paid, save — or 404 when the period is unknown.
/// </summary>
public class MarkDuesItemPaidCommandHandlerTests
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = Substitute.For<IDuesPeriodRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_MarksTheItemPaidAndSaves()
    {
        // arrange — a distributed dues period with one item
        var period = DuesPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), Money.Of(500m));
        var item = period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());
        _duesPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(new MarkDuesItemPaidCommand(period.Id, item.Id), CancellationToken.None);

        // assert
        item.Status.Should().Be(BillingItemStatus.Paid);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _duesPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((DuesPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new MarkDuesItemPaidCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private MarkDuesItemPaidCommandHandler CreateHandler() => new(_duesPeriodRepository, _unitOfWork);
}
