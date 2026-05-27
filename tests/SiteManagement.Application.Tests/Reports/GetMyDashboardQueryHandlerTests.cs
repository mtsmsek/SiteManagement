using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Messaging.Queries;
using SiteManagement.Application.Reports.Queries.GetMyDashboard;
using SiteManagement.Application.Shared.Exceptions;

namespace SiteManagement.Application.Tests.Reports;

/// <summary>
/// Unit tests for <see cref="GetMyDashboardQueryHandler"/>: it composes the
/// resident's summary from the billing + messaging read sides, scoped to the
/// caller, and refuses a caller with no linked resident.
/// </summary>
public class GetMyDashboardQueryHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IBillingQueries _billingQueries = Substitute.For<IBillingQueries>();
    private readonly IMessagingQueries _messagingQueries = Substitute.For<IMessagingQueries>();

    [Fact]
    public async Task Handle_ComposesOutstandingCreditAndUnreadForTheCurrentResident()
    {
        // arrange — one unpaid (500) + one paid (300) bill, 75 credit, two threads with 3 unread total
        var residentId = Guid.NewGuid();
        _currentUser.ResidentId.Returns(residentId);
        IReadOnlyList<ResidentBillDto> bills = new List<ResidentBillDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Dues", "2026-01", "Dues", 500m, "Unpaid"),
            new(Guid.NewGuid(), Guid.NewGuid(), "Utility", "2026-01", "Electricity", 300m, "Paid"),
        };
        _billingQueries.ListResidentBillsAsync(residentId, Arg.Any<CancellationToken>()).Returns(bills);
        _billingQueries.GetResidentCreditAsync(residentId, Arg.Any<CancellationToken>()).Returns(75m);
        IReadOnlyList<ConversationListItemDto> conversations = new List<ConversationListItemDto>
        {
            new(Guid.NewGuid(), residentId, "A", 2, DateTime.UtcNow, 0, 2),
            new(Guid.NewGuid(), residentId, "B", 1, DateTime.UtcNow, 0, 1),
        };
        _messagingQueries.ListForResidentAsync(residentId, Arg.Any<CancellationToken>()).Returns(conversations);
        var sut = new GetMyDashboardQueryHandler(_currentUser, _billingQueries, _messagingQueries);

        // act
        var result = await sut.Handle(new GetMyDashboardQuery(), CancellationToken.None);

        // assert
        result.TotalOutstanding.Should().Be(500m);
        result.UnpaidCount.Should().Be(1);
        result.TotalCredit.Should().Be(75m);
        result.UnreadMessages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WhenCallerHasNoResidentId_Throws()
    {
        // arrange
        _currentUser.ResidentId.Returns((Guid?)null);
        var sut = new GetMyDashboardQueryHandler(_currentUser, _billingQueries, _messagingQueries);

        // act
        var act = () => sut.Handle(new GetMyDashboardQuery(), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
    }
}
