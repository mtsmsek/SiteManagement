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
/// One end-to-end pass over the whole admin billing journey, mirroring the steps
/// a user takes in the UI: log in, build a site → block → apartment, assign a
/// resident, open + distribute a dues period, pay the item by card, then correct
/// the amount down so the resident gains credit — and see that credit applied
/// automatically to the next period's bill. A single test that proves the parts
/// wire together (the focused failure paths live in the other suites).
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class AdminBillingJourneyTests(PostgresFixture postgres) : IAsyncLifetime
{
    private const string ValidTc = "10000000146";
    private const string TestApiKey = "e2e-payment-key";

    private readonly PostgresFixture _postgres = postgres;
    private readonly WireMockServer _paymentStub = WireMockServer.Start();
    private CustomWebApplicationFactory _factory = null!;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory(_postgres, _paymentStub.Url, TestApiKey);
        await _factory.ResetDomainDataAsync();
        // The fake gateway approves every charge in this journey.
        _paymentStub
            .Given(Request.Create().WithPath("/api/payments").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new
            {
                transactionId = Guid.NewGuid(),
                status = "Succeeded",
                failureReason = (string?)null,
            }));
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
    /// Walks the full happy path the admin clicks through, asserting the state at
    /// each milestone the UI shows: item paid, then credited on correction, then
    /// auto-settled next period.
    /// </summary>
    [Fact]
    public async Task FullJourney_Login_Build_Distribute_Pay_Correct_Credit_AutoApply()
    {
        // arrange — log in (the "giriş")
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);

        // act + assert — build the property graph (site → block → apartment)
        var siteId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/sites", new { name = "Lavender Heights", address = "Cumhuriyet Mah. No:7" }));
        var blockId = await ReadIdAsync(await client.PostAsJsonAsync(
            $"/api/sites/{siteId}/blocks", new { name = "A" }));
        var apartmentId = await ReadIdAsync(await client.PostAsJsonAsync(
            $"/api/blocks/{blockId}/apartments", new { number = 1, floor = 1, type = "2+1" }));

        // assign a resident — occupies the apartment so distribution has a target
        var residentId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/residents",
            new { tcNo = ValidTc, firstName = "Ada", lastName = "Lovelace", email = "ada@e2e.local", phone = "05321234567" }));
        (await client.PostAsJsonAsync(
            "/api/assignments",
            new { apartmentId, residentId, tenantType = 0, startDate = "2026-01-01" }))
            .EnsureSuccessStatusCode();

        // open + distribute a dues period at 500
        var firstPeriodId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/dues", new { siteId, year = 2026, month = 1, perApartmentAmount = 500m }));
        (await client.PostAsync($"/api/dues/{firstPeriodId}/distribute", content: null)).EnsureSuccessStatusCode();
        var itemId = (await ItemsAsync(client, firstPeriodId)).Single().ItemId;

        // pay the item by card (the "cart") — gateway approves, item becomes Paid
        (await client.PostAsJsonAsync(
            $"/api/dues/{firstPeriodId}/items/{itemId}/pay-by-card",
            new { cardNumber = "4242424242424242", cvv = "123", expiryYear = 2030, expiryMonth = 12 }))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await ItemsAsync(client, firstPeriodId)).Single().Status.Should().Be("Paid");

        // correct the amount down to 100 — the resident over-paid by 400
        (await client.PutAsJsonAsync($"/api/dues/{firstPeriodId}", new { perApartmentAmount = 100m }))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        var debt = await client.GetFromJsonAsync<DebtSummary>(
            $"/api/dues/sites/{siteId}/debt-summary", AuthFlow.Json);
        debt!.TotalCredit.Should().Be(400m);

        // bill the same resident next month at 100 — credit settles it with no charge
        var secondPeriodId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/dues", new { siteId, year = 2026, month = 2, perApartmentAmount = 100m }));
        (await client.PostAsync($"/api/dues/{secondPeriodId}/distribute", content: null)).EnsureSuccessStatusCode();

        // assert — new item auto-paid from credit, balance drops to 300
        (await ItemsAsync(client, secondPeriodId)).Single().Status.Should().Be("Paid");
        var debtAfter = await client.GetFromJsonAsync<DebtSummary>(
            $"/api/dues/sites/{siteId}/debt-summary", AuthFlow.Json);
        debtAfter!.TotalCredit.Should().Be(300m);
    }

    private static async Task<List<PeriodItem>> ItemsAsync(HttpClient client, Guid duesPeriodId)
        => (await client.GetFromJsonAsync<List<PeriodItem>>($"/api/dues/{duesPeriodId}/items", AuthFlow.Json))!;

    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.EnumerateObject().First().Value.GetGuid();
    }

    private sealed record PeriodItem(Guid ItemId, decimal Amount, string Status);

    private sealed record DebtSummary(
        Guid SiteId,
        decimal TotalAccrued,
        decimal TotalCollected,
        decimal TotalOutstanding,
        decimal TotalCredit);
}
