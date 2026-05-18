using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Auth.Commands.Login;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tests.Doubles;

namespace SiteManagement.Application.Tests.Auth;

/// <summary>
/// Unit tests for <see cref="LoginCommandHandler"/>. The handler depends only
/// on Application-layer ports, so we can mock everything with NSubstitute.
/// </summary>
public class LoginCommandHandlerTests
{
    private readonly IUserAuthService _userAuth = Substitute.For<IUserAuthService>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IRefreshTokenStore _refreshStore = Substitute.For<IRefreshTokenStore>();

    /// <summary>Wrong email or password results in a 401 (no user enumeration).</summary>
    [Fact]
    public async Task Handle_WithInvalidCredentials_Throws401()
    {
        // arrange
        _userAuth.AuthenticateAsync(AuthDoubles.DefaultEmail, AuthDoubles.DefaultPassword, Arg.Any<CancellationToken>())
            .Returns((AuthenticatedUser?)null);
        var sut = CreateHandler();
        var cmd = new LoginCommand(AuthDoubles.DefaultEmail, AuthDoubles.DefaultPassword);

        // act
        var act = () => sut.Handle(cmd, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    /// <summary>Correct credentials produce tokens and persist the refresh token.</summary>
    [Fact]
    public async Task Handle_WithValidCredentials_IssuesTokensAndStoresRefresh()
    {
        // arrange
        var user = AuthDoubles.SampleUser();
        var expectedTokens = AuthDoubles.SampleTokens();

        _userAuth.AuthenticateAsync(user.Email, AuthDoubles.DefaultPassword, Arg.Any<CancellationToken>())
            .Returns(user);
        _tokenService.IssueTokens(user.Id, user.Email, Arg.Any<IEnumerable<string>>(), user.ResidentId)
            .Returns(expectedTokens);

        var sut = CreateHandler();
        var cmd = new LoginCommand(user.Email, AuthDoubles.DefaultPassword);

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

    private LoginCommandHandler CreateHandler() => new(_userAuth, _tokenService, _refreshStore);
}
