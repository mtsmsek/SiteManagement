using Testcontainers.PostgreSql;

namespace SiteManagement.E2E.Tests.Infrastructure;

/// <summary>
/// xunit collection fixture that brings up a single throw-away Postgres
/// container shared by every test in the <see cref="ApiCollection"/>
/// collection. The container starts once per test run, EF migrations
/// apply on the first <see cref="CustomWebApplicationFactory"/> request,
/// and the container is disposed at the end.
/// </summary>
/// <remarks>
/// We deliberately use a fresh container per test session (not per test
/// class) so the CI run cost stays low — bringing up Postgres takes
/// ~3-5 seconds. Per-test isolation is achieved by truncating the data
/// tables between tests via <see cref="ResetAsync"/>; the Identity
/// bootstrap admin is re-seeded by the application startup hook.
/// </remarks>
public sealed class PostgresFixture : IAsyncLifetime
{
    private const string DatabaseName = "sitemanagement_tests";
    private const string Username = "sitemanagement_tests";
    private const string Password = "sitemanagement_tests_pw";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase(DatabaseName)
        .WithUsername(Username)
        .WithPassword(Password)
        .WithCleanUp(true)
        .Build();

    /// <summary>Connection string the test web factory hands to <c>AppDbContext</c>.</summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <inheritdoc />
    public Task InitializeAsync() => _container.StartAsync();

    /// <inheritdoc />
    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}
