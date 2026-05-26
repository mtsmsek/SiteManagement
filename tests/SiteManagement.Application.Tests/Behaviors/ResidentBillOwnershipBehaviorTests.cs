using FluentAssertions;
using MediatR;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Behaviors;
using SiteManagement.Application.Billing;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Shared.Exceptions;

namespace SiteManagement.Application.Tests.Behaviors;

/// <summary>
/// Verifies the resource-ownership gate: an <see cref="IOwnedBillItemRequest"/>
/// only proceeds when the targeted item is one of the caller's own bills;
/// everything else (a foreign item, no resident id) is denied, and non-item
/// requests pass straight through untouched.
/// </summary>
public class ResidentBillOwnershipBehaviorTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IBillingQueries _billingQueries = Substitute.For<IBillingQueries>();

    [Fact]
    public async Task OwnedItem_IsAllowed_WhenItIsAmongTheCallersBills()
    {
        // arrange
        var residentId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        _currentUser.ResidentId.Returns(residentId);
        _billingQueries.ListResidentBillsAsync(residentId, Arg.Any<CancellationToken>())
            .Returns(new[] { Bill(itemId) });
        var (next, wasCalled) = TrackedNext();

        // act
        await Behavior().Handle(new OwnedRequest(itemId), next, CancellationToken.None);

        // assert
        wasCalled().Should().BeTrue();
    }

    [Fact]
    public async Task OwnedItem_IsDenied_WhenItIsNotAmongTheCallersBills()
    {
        // arrange — caller owns a different item
        var residentId = Guid.NewGuid();
        _currentUser.ResidentId.Returns(residentId);
        _billingQueries.ListResidentBillsAsync(residentId, Arg.Any<CancellationToken>())
            .Returns(new[] { Bill(Guid.NewGuid()) });
        var (next, wasCalled) = TrackedNext();

        // act
        var act = () => Behavior().Handle(new OwnedRequest(Guid.NewGuid()), next, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
        wasCalled().Should().BeFalse();
    }

    [Fact]
    public async Task OwnedItem_IsDenied_WhenCallerHasNoResidentId()
    {
        // arrange
        var (next, wasCalled) = TrackedNext();

        // act
        var act = () => Behavior().Handle(new OwnedRequest(Guid.NewGuid()), next, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
        wasCalled().Should().BeFalse();
    }

    [Fact]
    public async Task NonItemRequest_PassesThrough_WithoutCheckingBills()
    {
        // arrange
        var (next, wasCalled) = TrackedNext();

        // act
        await BehaviorFor<PlainRequest>().Handle(new PlainRequest(), next, CancellationToken.None);

        // assert
        wasCalled().Should().BeTrue();
        await _billingQueries.DidNotReceive().ListResidentBillsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    private ResidentBillOwnershipBehavior<OwnedRequest, Unit> Behavior() => new(_currentUser, _billingQueries);

    private ResidentBillOwnershipBehavior<T, Unit> BehaviorFor<T>() where T : notnull => new(_currentUser, _billingQueries);

    private static ResidentBillDto Bill(Guid itemId)
        => new(itemId, Guid.NewGuid(), BillingItemKinds.Dues, "2026-01", BillingItemKinds.Dues, 500m, "Unpaid");

    private static (RequestHandlerDelegate<Unit> Next, Func<bool> WasCalled) TrackedNext()
    {
        var called = false;
        RequestHandlerDelegate<Unit> next = _ =>
        {
            called = true;
            return Task.FromResult(Unit.Value);
        };
        return (next, () => called);
    }

    private sealed record OwnedRequest(Guid ItemId) : IRequest<Unit>, IOwnedBillItemRequest;

    private sealed record PlainRequest : IRequest<Unit>;
}
