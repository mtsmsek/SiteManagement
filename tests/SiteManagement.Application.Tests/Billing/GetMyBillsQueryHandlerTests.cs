using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Billing.Queries.GetMyBills;
using SiteManagement.Application.Shared.Exceptions;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="GetMyBillsQueryHandler"/>: it scopes the read to the
/// current resident's id (never the request) and refuses a caller with no
/// linked resident.
/// </summary>
public class GetMyBillsQueryHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IBillingQueries _billingQueries = Substitute.For<IBillingQueries>();

    [Fact]
    public async Task Handle_ReturnsBillsScopedToTheCurrentResident()
    {
        // arrange
        var residentId = Guid.NewGuid();
        _currentUser.ResidentId.Returns(residentId);
        IReadOnlyList<ResidentBillDto> bills = new List<ResidentBillDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), BillingItemKinds.Dues, "2026-01", BillingItemKinds.Dues, 500m, "Unpaid"),
        };
        _billingQueries.ListResidentBillsAsync(residentId, Arg.Any<CancellationToken>()).Returns(bills);
        var sut = new GetMyBillsQueryHandler(_currentUser, _billingQueries);

        // act
        var result = await sut.Handle(new GetMyBillsQuery(), CancellationToken.None);

        // assert
        result.Should().BeSameAs(bills);
        await _billingQueries.Received(1).ListResidentBillsAsync(residentId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCallerHasNoResidentId_Throws()
    {
        // arrange
        _currentUser.ResidentId.Returns((Guid?)null);
        var sut = new GetMyBillsQueryHandler(_currentUser, _billingQueries);

        // act
        var act = () => sut.Handle(new GetMyBillsQuery(), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
    }
}
