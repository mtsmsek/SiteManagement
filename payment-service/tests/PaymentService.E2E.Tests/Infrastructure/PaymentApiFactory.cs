using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace PaymentService.E2E.Tests.Infrastructure;

/// <summary>
/// In-process <see cref="WebApplicationFactory{TEntryPoint}"/> for the
/// PaymentService API, pointed at the shared <see cref="MongoFixture"/>'s
/// container. Overrides the Mongo connection string and leaves the API key
/// unset (the middleware no-ops when empty), so tests hit the real charge
/// pipeline — controller → processor → Mongo — over HTTP without ceremony.
/// </summary>
public sealed class PaymentApiFactory(MongoFixture mongo) : WebApplicationFactory<Program>
{
    private readonly MongoFixture _mongo = mongo;

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Mongo:ConnectionString"] = _mongo.ConnectionString,
                // A unique database per factory keeps the seeded demo card/account
                // and any written transactions isolated from a developer's local data.
                ["Mongo:Database"] = $"payments_e2e_{Guid.NewGuid():N}",
                // No service API key in tests: the middleware skips the check when unset.
                ["ApiKey:Value"] = string.Empty,
            });
        });
    }
}
