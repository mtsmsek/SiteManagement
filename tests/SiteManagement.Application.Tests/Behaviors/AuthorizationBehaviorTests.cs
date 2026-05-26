using FluentAssertions;
using MediatR;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Application.Behaviors;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Identity;

namespace SiteManagement.Application.Tests.Behaviors;

/// <summary>
/// Verifies the central authorization gate: a request's marker
/// (<see cref="IAdminRequest"/> / <see cref="IResidentRequest"/> /
/// <see cref="IPublicRequest"/>) decides access, and an unmarked request is
/// denied (fail closed). Ownership is out of scope here — that lives in handlers.
/// </summary>
public class AuthorizationBehaviorTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();

    [Fact]
    public async Task PublicRequest_IsAllowed_EvenWithoutAnyRole()
    {
        // arrange — caller has no roles at all
        var (next, wasCalled) = TrackedNext();

        // act
        await new AuthorizationBehavior<PublicRequest, Unit>(_currentUser)
            .Handle(new PublicRequest(), next, CancellationToken.None);

        // assert
        wasCalled().Should().BeTrue();
    }

    [Fact]
    public async Task AdminRequest_IsAllowed_WhenCallerIsAdmin()
    {
        // arrange
        _currentUser.IsInRole(Roles.Admin).Returns(true);
        var (next, wasCalled) = TrackedNext();

        // act
        await new AuthorizationBehavior<AdminRequest, Unit>(_currentUser)
            .Handle(new AdminRequest(), next, CancellationToken.None);

        // assert
        wasCalled().Should().BeTrue();
    }

    [Fact]
    public async Task AdminRequest_IsDenied_WhenCallerIsNotAdmin()
    {
        // arrange — not in the Admin role
        var (next, wasCalled) = TrackedNext();

        // act
        var act = () => new AuthorizationBehavior<AdminRequest, Unit>(_currentUser)
            .Handle(new AdminRequest(), next, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
        wasCalled().Should().BeFalse();
    }

    [Fact]
    public async Task ResidentRequest_IsAllowed_WhenCallerIsResidentWithResidentId()
    {
        // arrange
        _currentUser.IsInRole(Roles.Resident).Returns(true);
        _currentUser.ResidentId.Returns(Guid.NewGuid());
        var (next, wasCalled) = TrackedNext();

        // act
        await new AuthorizationBehavior<ResidentRequest, Unit>(_currentUser)
            .Handle(new ResidentRequest(), next, CancellationToken.None);

        // assert
        wasCalled().Should().BeTrue();
    }

    [Fact]
    public async Task ResidentRequest_IsDenied_WhenCallerLacksResidentRole()
    {
        // arrange — has a resident id but not the role
        _currentUser.ResidentId.Returns(Guid.NewGuid());
        var (next, wasCalled) = TrackedNext();

        // act
        var act = () => new AuthorizationBehavior<ResidentRequest, Unit>(_currentUser)
            .Handle(new ResidentRequest(), next, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
        wasCalled().Should().BeFalse();
    }

    [Fact]
    public async Task ResidentRequest_IsDenied_WhenResidentIdIsMissing()
    {
        // arrange — in the role but no linked resident id
        _currentUser.IsInRole(Roles.Resident).Returns(true);
        var (next, wasCalled) = TrackedNext();

        // act
        var act = () => new AuthorizationBehavior<ResidentRequest, Unit>(_currentUser)
            .Handle(new ResidentRequest(), next, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
        wasCalled().Should().BeFalse();
    }

    [Fact]
    public async Task UnmarkedRequest_IsDenied_FailingClosed()
    {
        // arrange — an admin caller, but the request declares no requirement
        _currentUser.IsInRole(Roles.Admin).Returns(true);
        var (next, wasCalled) = TrackedNext();

        // act
        var act = () => new AuthorizationBehavior<UnmarkedRequest, Unit>(_currentUser)
            .Handle(new UnmarkedRequest(), next, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<UnauthorizedActionException>();
        wasCalled().Should().BeFalse();
    }

    /// <summary>A handler delegate that records whether it was invoked.</summary>
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

    private sealed record PublicRequest : IRequest<Unit>, IPublicRequest;

    private sealed record AdminRequest : IRequest<Unit>, IAdminRequest;

    private sealed record ResidentRequest : IRequest<Unit>, IResidentRequest;

    private sealed record UnmarkedRequest : IRequest<Unit>;
}
