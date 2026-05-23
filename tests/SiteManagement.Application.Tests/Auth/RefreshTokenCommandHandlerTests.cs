using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Auth.Commands.Refresh;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tests.Doubles;

namespace SiteManagement.Application.Tests.Auth;

/// <summary>
/// Unit tests for <see cref="RefreshTokenCommandHandler"/>: consume the supplied
/// refresh token, issue a fresh pair, and store it — or 401 when the token is
/// unknown, expired, or already consumed.
/// </summary>
public class RefreshTokenCommandHandlerTests
{
    private readonly IRefreshTokenStore _refreshStore = Substitute.For<IRefreshTokenStore>();
    private readonly IUserAuthService _userAuth = Substitute.For<IUserAuthService>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();

    [Fact]
    public async Task Handle_WithValidToken_IssuesTokensAndStoresRefresh()
    {
        // arrange
        var user = AuthDoubles.SampleUser();
        var expectedTokens = AuthDoubles.SampleTokens();

        _refreshStore.ConsumeAsync(AuthDoubles.SampleRefreshToken, Arg.Any<CancellationToken>()).Returns(user.Id);
        _userAuth.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _tokenService.IssueTokens(user.Id, user.Email, user.Roles, user.ResidentId).Returns(expectedTokens);

        var sut = CreateHandler();
        var cmd = new RefreshTokenCommand(AuthDoubles.SampleRefreshToken);

        // act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // assert
        result.Should().Be(expectedTokens);
        await _refreshStore.Received(1).StoreAsync(
            user.Id,
            expectedTokens.RefreshToken,
            expectedTokens.RefreshTokenExpiresAtUtc,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownToken_Throws401()
    {
        // arrange
        _refreshStore.ConsumeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Guid?)null);
        var sut = CreateHandler();
        var cmd = new RefreshTokenCommand(AuthDoubles.SampleRefreshToken);

        // act
        var act = () => sut.Handle(cmd, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    private RefreshTokenCommandHandler CreateHandler() => new(_refreshStore, _userAuth, _tokenService);
}
