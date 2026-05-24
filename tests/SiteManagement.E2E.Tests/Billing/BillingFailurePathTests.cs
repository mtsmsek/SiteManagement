using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Billing;

/// <summary>
/// Failure paths for the Billing workflow: distributing into a closed period
/// and distributing with no occupied apartments are both rejected, so the
/// domain invariants surface as HTTP errors rather than silent no-ops.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class BillingFailurePathTests(PostgresFixture postgres) : IAsyncLifetime
{
    private const string ValidTc = "10000000146";

    private readonly CustomWebApplicationFactory _factory = new(postgres);

    /// <inheritdoc />
    public Task InitializeAsync() => _factory.ResetDomainDataAsync();

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>Distributing dues into an already-closed period is a conflict.</summary>
    [Fact]
    public async Task DistributeDues_IntoClosedPeriod_IsConflict()
    {
        // arrange — an occupied apartment, then a dues period distributed + closed
        var client = await CreateAdminClientAsync();
        var siteId = await SeedOccupiedApartmentAsync(client);
        var duesPeriodId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/dues", new { siteId, year = 2026, month = 1, perApartmentAmount = 500m }));
        (await client.PostAsync($"/api/dues/{duesPeriodId}/distribute", content: null)).EnsureSuccessStatusCode();
        (await client.PostAsync($"/api/dues/{duesPeriodId}/close", content: null)).EnsureSuccessStatusCode();

        // act — distribute again after close
        var response = await client.PostAsync($"/api/dues/{duesPeriodId}/distribute", content: null);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>Distributing a utility bill across zero occupied apartments is a conflict.</summary>
    [Fact]
    public async Task DistributeUtilityBill_WithNoOccupants_IsConflict()
    {
        // arrange — a site with no occupied apartments, plus an open utility period
        var client = await CreateAdminClientAsync();
        var siteId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/sites", new { name = "Empty Towers", address = "Bos Mah." }));
        var periodId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/utility-bills", new { siteId, year = 2026, month = 1, utilityType = 0, totalAmount = 300m }));

        // act — nothing to split across
        var response = await client.PostAsync($"/api/utility-bills/{periodId}/distribute", content: null);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);
        return client;
    }

    /// <summary>Builds a site with one apartment occupied by an owner; returns the site id.</summary>
    private static async Task<Guid> SeedOccupiedApartmentAsync(HttpClient client)
    {
        var siteId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/sites", new { name = "Lavender Heights", address = "Cumhuriyet Mah. No:7" }));
        var blockId = await ReadIdAsync(await client.PostAsJsonAsync(
            $"/api/sites/{siteId}/blocks", new { name = "A" }));
        var apartmentId = await ReadIdAsync(await client.PostAsJsonAsync(
            $"/api/blocks/{blockId}/apartments", new { number = 1, floor = 1, type = "2+1" }));
        var residentId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/residents",
            new { tcNo = ValidTc, firstName = "Ada", lastName = "Lovelace", email = "ada@e2e.local", phone = "05321234567" }));
        (await client.PostAsJsonAsync(
            "/api/assignments",
            new { apartmentId, residentId, tenantType = 0, startDate = "2026-01-01" }))
            .EnsureSuccessStatusCode();
        return siteId;
    }

    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.EnumerateObject().First().Value.GetGuid();
    }
}
