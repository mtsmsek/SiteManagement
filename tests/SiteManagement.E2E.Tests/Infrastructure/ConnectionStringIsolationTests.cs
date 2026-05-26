using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Infrastructure.Persistence;

namespace SiteManagement.E2E.Tests.Infrastructure;

/// <summary>
/// Guardrail: the test host must talk to the throw-away Testcontainer, never a
/// connection string baked into appsettings/env (e.g. a developer's local
/// <c>docker compose</c> Postgres on :5432). Regression test for the W4 close-out
/// incident where the connection string was resolved eagerly at registration —
/// before the factory's in-memory override applied — so E2E ran against the
/// compose DB and truncated its data + bootstrap admin.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ConnectionStringIsolationTests(PostgresFixture postgres)
{
    private readonly PostgresFixture _postgres = postgres;

    /// <summary>
    /// The resolved <see cref="AppDbContext"/> must use the fixture's container
    /// connection string, proving the factory's override wins over ambient config.
    /// </summary>
    [Fact]
    public void TestHost_ResolvesFixtureConnectionString_NotAmbientConfig()
    {
        // arrange
        using var factory = new CustomWebApplicationFactory(_postgres);

        // act
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var actual = db.Database.GetConnectionString();

        // assert
        actual.Should().Be(_postgres.ConnectionString);
    }
}
