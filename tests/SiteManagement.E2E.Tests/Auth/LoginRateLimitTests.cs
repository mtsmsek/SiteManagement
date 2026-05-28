using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Auth;

/// <summary>
/// Guards the login rate-limit policy: a small burst of bad credentials from
/// the same partition (IP) must trip the limiter into a 429. Each test gets
/// its own <see cref="CustomWebApplicationFactory"/> and therefore its own
/// in-process limiter state, so this case is isolated from the rest of the
/// suite.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class LoginRateLimitTests(PostgresFixture postgres) : IAsyncLifetime
{
    private readonly PostgresFixture _postgres = postgres;
    private CustomWebApplicationFactory _factory = null!;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory(_postgres);
        await _factory.ResetDomainDataAsync();
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task LoginAttempts_BeyondPolicyLimit_Return429()
    {
        // arrange — the fixed window allows 5 attempts/minute per IP partition;
        // we go to 7 so even with one or two slipping through unrelated traffic
        // the final shots land on the limiter.
        var client = _factory.CreateClient();
        var credentials = new { email = "ghost@e2e.local", password = "definitely-wrong" };

        // act
        HttpResponseMessage? last = null;
        for (var i = 0; i < 7; i++)
        {
            last = await client.PostAsJsonAsync("/api/auth/login", credentials);
        }

        // assert — the final response is the policy rejection
        last!.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}
