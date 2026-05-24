using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiteManagement.Infrastructure.Auth;
using SiteManagement.Infrastructure.Persistence;

namespace SiteManagement.E2E.Tests.Infrastructure;

/// <summary>
/// In-process <see cref="WebApplicationFactory{TEntryPoint}"/> bound to the
/// shared <see cref="PostgresFixture"/>. Overrides the connection string,
/// bootstrap admin and SMTP settings via <c>InMemoryCollection</c> so each
/// test session gets a deterministic environment regardless of what the
/// developer has on their machine.
/// </summary>
public sealed class CustomWebApplicationFactory(
    PostgresFixture postgres,
    string? paymentServiceBaseUrl = null,
    string? paymentServiceApiKey = null) : WebApplicationFactory<Program>
{
    /// <summary>Email of the bootstrap admin seeded into the test database on first request.</summary>
    public const string BootstrapAdminEmail = "admin@e2e.local";

    /// <summary>Password of the bootstrap admin (meets the Identity policy).</summary>
    public const string BootstrapAdminPassword = "E2e-T3st-P@ss";

    private const string TestJwtKey = "e2e-only-secret-not-used-in-production-please";

    private readonly PostgresFixture _postgres = postgres;
    private readonly string? _paymentServiceBaseUrl = paymentServiceBaseUrl;
    private readonly string? _paymentServiceApiKey = paymentServiceApiKey;

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.ConnectionString,
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:Issuer"] = "SiteManagement.E2E",
                ["Jwt:Audience"] = "SiteManagement.E2E.Clients",
                ["Jwt:AccessTokenMinutes"] = "60",
                ["Jwt:RefreshTokenDays"] = "14",
                ["Admin:BootstrapEmail"] = BootstrapAdminEmail,
                ["Admin:BootstrapPassword"] = BootstrapAdminPassword,
                ["Admin:BootstrapFullName"] = "E2E Bootstrap Admin",
                // MailHog isn't available in CI; redirect to a no-op port so
                // SmtpEmailSender stays loaded but ResidentEmailFakeSender
                // (registered below) takes over on the test side.
                ["Smtp:Host"] = "localhost",
                ["Smtp:Port"] = "2525",
                // Keep the background outbox poller effectively idle during tests
                // so it never races an explicit ProcessOutboxAsync call.
                ["Outbox:PollSeconds"] = "3600",
            };

            // Point the payment gateway at the test's stub server when one is
            // supplied (the pay-by-card E2E); other suites never hit the gateway
            // so they leave it unset.
            if (_paymentServiceBaseUrl is not null)
            {
                settings["PaymentService:BaseUrl"] = _paymentServiceBaseUrl;
                settings["PaymentService:ApiKey"] = _paymentServiceApiKey ?? string.Empty;
            }

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            // Swap the real SMTP sender for a fake that records messages in
            // memory so resident-registration tests can assert on the
            // generated welcome email without an SMTP server.
            services.RemoveAll<SiteManagement.Application.Abstractions.Email.IEmailSender>();
            services.AddSingleton<RecordingEmailSender>();
            services.AddSingleton<SiteManagement.Application.Abstractions.Email.IEmailSender>(
                sp => sp.GetRequiredService<RecordingEmailSender>());
        });
    }

    /// <summary>
    /// Truncates the domain tables between tests so each test starts on a
    /// known-clean DB. Identity tables stay intact — the bootstrap admin
    /// seed runs once on first request.
    /// </summary>
    public async Task ResetDomainDataAsync(CancellationToken ct = default)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"DuesItems\", \"DuesPeriods\", \"UtilityBillItems\", \"UtilityBillPeriods\", "
            + "\"ApartmentAssignments\", \"ResidentVehicles\", \"Apartments\", \"Blocks\", \"Sites\", \"Residents\", "
            + "\"OutboxMessages\" "
            + "RESTART IDENTITY CASCADE;",
            cancellationToken: ct);

        // The bootstrap admin lives in the Users table and stays — login still works.
        // Residents created via tests are wiped, but the linked AppUser rows aren't:
        // delete any non-bootstrap users so e-mail uniqueness doesn't trip the next test.
        await db.Database.ExecuteSqlRawAsync(
            """
            DELETE FROM "UserRoles" WHERE "UserId" IN
                (SELECT "Id" FROM "Users" WHERE "Email" <> {0});
            DELETE FROM "Users" WHERE "Email" <> {0};
            """,
            parameters: new object[] { BootstrapAdminEmail },
            cancellationToken: ct);

        // Reset in-memory refresh token store as well: it's a singleton,
        // tokens issued by previous tests would otherwise carry over.
        var refreshStore = scope.ServiceProvider
            .GetRequiredService<SiteManagement.Application.Abstractions.Auth.IRefreshTokenStore>();
        if (refreshStore is InMemoryRefreshTokenStore resetable)
        {
            resetable.Clear();
        }
    }

    /// <summary>Convenience: gives every test a recording email sender already plugged in.</summary>
    public RecordingEmailSender Emails => Services.GetRequiredService<RecordingEmailSender>();

    /// <summary>
    /// Runs the outbox processor once, deterministically, so tests can assert
    /// after-commit delivery without waiting on the background poller's timer.
    /// </summary>
    public async Task<int> ProcessOutboxAsync(CancellationToken ct = default)
    {
        using var scope = Services.CreateScope();
        var processor = scope.ServiceProvider
            .GetRequiredService<SiteManagement.Application.Abstractions.Events.IOutboxProcessor>();
        return await processor.ProcessPendingAsync(ct);
    }
}
