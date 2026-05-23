using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Billing;

/// <summary>
/// End-to-end smoke for the admin's Billing workflow: with one occupied
/// apartment in place, opening a period and distributing it produces a single
/// per-occupant line that the new period-items read endpoints expose with the
/// apartment label, resident name, amount and payment status the UI needs.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class BillingFlowTests(PostgresFixture postgres) : IAsyncLifetime
{
    // Algorithm-valid synthetic TC number reused from the Residency doubles.
    private const string ValidTc = "10000000146";

    private readonly CustomWebApplicationFactory _factory = new(postgres);

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _factory.ResetDomainDataAsync();
        _factory.Emails.Clear();
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Distributing a dues period bills every occupied apartment; the items
    /// endpoint surfaces that single line with the "A-1" apartment label, the
    /// occupant's full name, the per-apartment amount and an Unpaid status.
    /// </summary>
    [Fact]
    public async Task DistributeDues_ThenListItems_ReturnsPerOccupantLine()
    {
        // arrange — one occupied apartment owned by a known resident
        var client = await CreateAdminClientAsync();
        var siteId = await SeedOccupiedApartmentAsync(client);

        // act — open + distribute a dues period, then read its items
        var duesPeriodId = await OpenDuesPeriodAsync(client, siteId, perApartmentAmount: 500m);
        (await client.PostAsync($"/api/dues/{duesPeriodId}/distribute", content: null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        var items = await client.GetFromJsonAsync<List<PeriodItem>>(
            $"/api/dues/{duesPeriodId}/items", AuthFlow.Json);

        // assert — single line carrying the UI columns
        items.Should().ContainSingle();
        var item = items![0];
        item.Apartment.Should().Be("A-1");
        item.ResidentFullName.Should().Be("Ada Lovelace");
        item.Amount.Should().Be(500m);
        item.Status.Should().Be("Unpaid");
    }

    /// <summary>
    /// The utility-bill items endpoint mirrors the dues one: distributing a
    /// total across the single occupied apartment yields one line for that
    /// apartment carrying the full amount.
    /// </summary>
    [Fact]
    public async Task DistributeUtilityBill_ThenListItems_ReturnsPerOccupantLine()
    {
        // arrange
        var client = await CreateAdminClientAsync();
        var siteId = await SeedOccupiedApartmentAsync(client);

        // act — open + distribute an electricity bill, then read its items
        var periodId = await OpenUtilityBillPeriodAsync(client, siteId, totalAmount: 300m);
        (await client.PostAsync($"/api/utility-bills/{periodId}/distribute", content: null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        var items = await client.GetFromJsonAsync<List<PeriodItem>>(
            $"/api/utility-bills/{periodId}/items", AuthFlow.Json);

        // assert
        items.Should().ContainSingle();
        var item = items![0];
        item.Apartment.Should().Be("A-1");
        item.ResidentFullName.Should().Be("Ada Lovelace");
        item.Amount.Should().Be(300m);
        item.Status.Should().Be("Unpaid");
    }

    /// <summary>
    /// Paying a distributed dues item flips it to Paid and makes it count as
    /// collected in the site debt summary (outstanding drops to zero).
    /// </summary>
    [Fact]
    public async Task PayDuesItem_MarksItPaid_AndCountsAsCollected()
    {
        // arrange — distribute a dues period over the single occupied apartment
        var client = await CreateAdminClientAsync();
        var siteId = await SeedOccupiedApartmentAsync(client);
        var duesPeriodId = await OpenDuesPeriodAsync(client, siteId, perApartmentAmount: 500m);
        (await client.PostAsync($"/api/dues/{duesPeriodId}/distribute", content: null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        var items = await client.GetFromJsonAsync<List<PeriodItem>>(
            $"/api/dues/{duesPeriodId}/items", AuthFlow.Json);
        var itemId = items![0].ItemId;

        // act — pay that item
        var payResponse = await client.PostAsync(
            $"/api/dues/{duesPeriodId}/items/{itemId}/pay", content: null);

        // assert — 204, the item is Paid, and it now counts as collected
        payResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var afterItems = await client.GetFromJsonAsync<List<PeriodItem>>(
            $"/api/dues/{duesPeriodId}/items", AuthFlow.Json);
        afterItems!.Single().Status.Should().Be("Paid");
        var debt = await client.GetFromJsonAsync<DebtSummary>(
            $"/api/dues/sites/{siteId}/debt-summary", AuthFlow.Json);
        debt!.TotalCollected.Should().Be(500m);
        debt.TotalOutstanding.Should().Be(0m);
    }

    /// <summary>
    /// Closing a dues period does not send the billing email inside the
    /// transaction; it queues an integration event in the outbox, and the
    /// processor delivers it after commit. Re-processing is idempotent.
    /// </summary>
    [Fact]
    public async Task CloseDuesPeriod_DefersNotificationToOutbox_ThenProcessorDelivers()
    {
        // arrange — a distributed dues period so closing has someone to notify
        var client = await CreateAdminClientAsync();
        var siteId = await SeedOccupiedApartmentAsync(client);
        var duesPeriodId = await OpenDuesPeriodAsync(client, siteId, perApartmentAmount: 500m);
        (await client.PostAsync($"/api/dues/{duesPeriodId}/distribute", content: null))
            .EnsureSuccessStatusCode();

        // act — close the period
        var closeResponse = await client.PostAsync($"/api/dues/{duesPeriodId}/close", content: null);

        // assert — close committed, but the billing email did NOT ride the transaction
        closeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _factory.Emails.BillingNotifications.Should().BeEmpty();

        // act — the outbox processor delivers it after commit
        var delivered = await _factory.ProcessOutboxAsync();

        // assert — exactly one notification reaches the occupant
        delivered.Should().BeGreaterThan(0);
        _factory.Emails.BillingNotifications.Should().ContainSingle()
            .Which.ToEmail.Should().Be("ada@e2e.local");

        // act + assert — re-processing delivers nothing new (idempotent)
        await _factory.ProcessOutboxAsync();
        _factory.Emails.BillingNotifications.Should().ContainSingle();
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);
        return client;
    }

    /// <summary>
    /// Builds a site with block "A" / apartment "1", registers a resident, and
    /// assigns them as owner — which occupies the apartment via the tenancy
    /// domain event. Returns the owning site id.
    /// </summary>
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
            new
            {
                tcNo = ValidTc,
                firstName = "Ada",
                lastName = "Lovelace",
                email = "ada@e2e.local",
                phone = "05321234567",
            }));

        // tenantType 0 = Owner; the assignment occupies the apartment.
        (await client.PostAsJsonAsync(
            "/api/assignments",
            new { apartmentId, residentId, tenantType = 0, startDate = "2026-01-01" }))
            .EnsureSuccessStatusCode();

        return siteId;
    }

    private static async Task<Guid> OpenDuesPeriodAsync(HttpClient client, Guid siteId, decimal perApartmentAmount)
    {
        var response = await client.PostAsJsonAsync(
            "/api/dues", new { siteId, year = 2026, month = 1, perApartmentAmount });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await ReadIdAsync(response);
    }

    private static async Task<Guid> OpenUtilityBillPeriodAsync(HttpClient client, Guid siteId, decimal totalAmount)
    {
        // utilityType 0 = Electricity.
        var response = await client.PostAsJsonAsync(
            "/api/utility-bills", new { siteId, year = 2026, month = 1, utilityType = 0, totalAmount });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await ReadIdAsync(response);
    }

    /// <summary>Reads the single Guid property out of a create/assign response, whatever its name.</summary>
    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.EnumerateObject().First().Value.GetGuid();
    }

    private sealed record PeriodItem(
        Guid ItemId,
        Guid ApartmentId,
        string Apartment,
        Guid ResidentId,
        string ResidentFullName,
        decimal Amount,
        string Status);

    private sealed record DebtSummary(
        Guid SiteId,
        decimal TotalAccrued,
        decimal TotalCollected,
        decimal TotalOutstanding);
}
