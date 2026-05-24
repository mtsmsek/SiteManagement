using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SiteManagement.E2E.Tests.Billing;

/// <summary>
/// End-to-end for the consumer side of the payment integration: the main API
/// charges a dues item by card through the real Refit client + Polly pipeline,
/// pointed at a stub Payment service (WireMock). Stubbing the gateway over real
/// HTTP keeps this a focused contract test — it proves the main API's flow
/// (idempotency key, 402-on-decline, item stays unpaid, clean failure when the
/// gateway is down) without depending on PaymentService's code; PaymentService's
/// own behaviour is proven by its dedicated E2E suite.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class PayByCardFlowTests(PostgresFixture postgres) : IAsyncLifetime
{
    private const string ValidTc = "10000000146";
    private const string ChargePath = "/api/payments";
    private const string TestApiKey = "e2e-payment-key";

    private readonly PostgresFixture _postgres = postgres;
    private readonly WireMockServer _paymentStub = WireMockServer.Start();
    private CustomWebApplicationFactory _factory = null!;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory(_postgres, _paymentStub.Url, TestApiKey);
        await _factory.ResetDomainDataAsync();
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        _paymentStub.Stop();
        _paymentStub.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// A successful charge flips the item to Paid: the main API posts to the
    /// gateway, gets a Succeeded result, and commits the paid state (204).
    /// </summary>
    [Fact]
    public async Task PayDuesItemByCard_WhenGatewayApproves_MarksItemPaid()
    {
        // arrange — stub the gateway to approve, and distribute a dues item
        StubCharge(StatusBody("Succeeded"));
        var client = await CreateAdminClientAsync();
        var (duesPeriodId, itemId) = await SeedDistributedDuesItemAsync(client);

        // act
        var response = await client.PostAsJsonAsync(
            $"/api/dues/{duesPeriodId}/items/{itemId}/pay-by-card", Card());

        // assert — 204 and the item is now Paid
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await ItemStatusAsync(client, duesPeriodId, itemId)).Should().Be("Paid");
    }

    /// <summary>
    /// A declined charge surfaces as 402 and leaves the item Unpaid: the failed
    /// external charge is not a domain-rule violation (409), and nothing is
    /// committed locally.
    /// </summary>
    [Fact]
    public async Task PayDuesItemByCard_WhenGatewayDeclines_Returns402_AndLeavesItemUnpaid()
    {
        // arrange — stub the gateway to decline (insufficient balance)
        StubCharge(StatusBody("Failed", "insufficient_balance"));
        var client = await CreateAdminClientAsync();
        var (duesPeriodId, itemId) = await SeedDistributedDuesItemAsync(client);

        // act
        var response = await client.PostAsJsonAsync(
            $"/api/dues/{duesPeriodId}/items/{itemId}/pay-by-card", Card());

        // assert — 402 Payment Required, item still Unpaid
        response.StatusCode.Should().Be(HttpStatusCode.PaymentRequired);
        (await ItemStatusAsync(client, duesPeriodId, itemId)).Should().Be("Unpaid");
    }

    /// <summary>
    /// The main API sends the deterministic idempotency key derived from the item
    /// id, so a repeat charge against the gateway resolves to the same charge —
    /// every request the gateway sees carries the same key.
    /// </summary>
    [Fact]
    public async Task PayDuesItemByCard_SendsDeterministicIdempotencyKey()
    {
        // arrange
        StubCharge(StatusBody("Succeeded"));
        var client = await CreateAdminClientAsync();
        var (duesPeriodId, itemId) = await SeedDistributedDuesItemAsync(client);

        // act — pay the same item twice
        await client.PostAsJsonAsync($"/api/dues/{duesPeriodId}/items/{itemId}/pay-by-card", Card());
        await client.PostAsJsonAsync($"/api/dues/{duesPeriodId}/items/{itemId}/pay-by-card", Card());

        // assert — both charges carried the item-derived idempotency key
        var keys = _paymentStub.LogEntries
            .Where(e => e.RequestMessage.Path == ChargePath)
            .Select(e => JsonDocument.Parse(e.RequestMessage.Body!).RootElement
                .GetProperty("idempotencyKey").GetString())
            .ToList();
        keys.Should().HaveCount(2);
        keys.Should().AllBe($"dues-item:{itemId}");
    }

    /// <summary>
    /// When the gateway is unreachable, the resilience pipeline exhausts its
    /// retries and the request fails cleanly (a 5xx, not a hang) while the item
    /// stays Unpaid — a down dependency degrades gracefully.
    /// </summary>
    [Fact]
    public async Task PayDuesItemByCard_WhenGatewayDown_FailsCleanly_AndLeavesItemUnpaid()
    {
        // arrange — distribute an item, then take the gateway down
        var client = await CreateAdminClientAsync();
        var (duesPeriodId, itemId) = await SeedDistributedDuesItemAsync(client);
        _paymentStub.Stop();

        // act
        var response = await client.PostAsJsonAsync(
            $"/api/dues/{duesPeriodId}/items/{itemId}/pay-by-card", Card());

        // assert — a clean server-side failure, not a 2xx, and the item is untouched
        ((int)response.StatusCode).Should().BeGreaterThanOrEqualTo(500);
        (await ItemStatusAsync(client, duesPeriodId, itemId)).Should().Be("Unpaid");
    }

    /// <summary>Configures the stub gateway's charge endpoint to return the given body with 200.</summary>
    private void StubCharge(object body)
        => _paymentStub
            .Given(Request.Create().WithPath(ChargePath).UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(body));

    private static object StatusBody(string status, string? failureReason = null) => new
    {
        transactionId = Guid.NewGuid(),
        status,
        failureReason,
    };

    private static object Card() => new
    {
        cardNumber = "4242424242424242",
        cvv = "123",
        expiryYear = 2030,
        expiryMonth = 12,
    };

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);
        return client;
    }

    /// <summary>Reads back one period item's payment status.</summary>
    private static async Task<string> ItemStatusAsync(HttpClient client, Guid duesPeriodId, Guid itemId)
    {
        var items = await client.GetFromJsonAsync<List<PeriodItem>>(
            $"/api/dues/{duesPeriodId}/items", AuthFlow.Json);
        return items!.Single(i => i.ItemId == itemId).Status;
    }

    /// <summary>
    /// Seeds an occupied apartment, opens + distributes a dues period, and
    /// returns the period id and the single distributed item's id.
    /// </summary>
    private static async Task<(Guid PeriodId, Guid ItemId)> SeedDistributedDuesItemAsync(HttpClient client)
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

        var periodId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/dues", new { siteId, year = 2026, month = 1, perApartmentAmount = 500m }));
        (await client.PostAsync($"/api/dues/{periodId}/distribute", content: null)).EnsureSuccessStatusCode();

        var items = await client.GetFromJsonAsync<List<PeriodItem>>(
            $"/api/dues/{periodId}/items", AuthFlow.Json);
        return (periodId, items!.Single().ItemId);
    }

    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.EnumerateObject().First().Value.GetGuid();
    }

    private sealed record PeriodItem(Guid ItemId, decimal Amount, string Status);
}
