using Testcontainers.MongoDb;

namespace PaymentService.E2E.Tests.Infrastructure;

/// <summary>
/// xunit collection fixture that brings up a single throw-away MongoDB
/// container shared by every test in the <see cref="PaymentApiCollection"/>
/// collection. The container starts once per test run and is disposed at the
/// end; per-test isolation comes from each charge using a distinct idempotency
/// key, so no data reset between tests is needed.
/// </summary>
public sealed class MongoFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container = new MongoDbBuilder()
        .WithImage("mongo:7")
        .WithCleanUp(true)
        .Build();

    /// <summary>Connection string the test web factory hands to the Mongo context.</summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <inheritdoc />
    public Task InitializeAsync() => _container.StartAsync();

    /// <inheritdoc />
    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}
