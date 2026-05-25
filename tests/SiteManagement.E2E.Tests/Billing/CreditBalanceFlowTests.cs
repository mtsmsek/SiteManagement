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
/// End-to-end for the over-payment credit flow: paying a dues item, then
/// correcting the period's amount down, credits the over-paid resident; the
/// credit surfaces in the site debt summary and is then consumed automatically
/// when the same resident is billed again — settling the new item without a
/// second charge.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class CreditBalanceFlowTests(PostgresFixture postgres) : IAsyncLifetime
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
        // Approve every charge in this suite.
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
    /// Pay a dues item at 500, then correct the period down to 100: the resident
    /// is credited the 400 over-payment, which shows in the debt summary and is
    /// then auto-applied to a fresh dues period — the new item lands Paid and the
    /// credit drops by the new amount.
    /// </summary>
    [Fact]
    public async Task Overpayment_IsCredited_ThenAutoAppliedToNextPeriod()
    {
        // arrange — one occupied apartment with a distributed, card-paid dues item at 500
        var client = await CreateAdminClientAsync();
        var siteId = await SeedOccupiedApartmentAsync(client);
        var firstPeriodId = await OpenDuesPeriodAsync(client, siteId, 500m);
        (await client.PostAsync($"/api/dues/{firstPeriodId}/distribute", content: null)).EnsureSuccessStatusCode();
        var firstItemId = (await ItemsAsync(client, firstPeriodId))[0].ItemId;
        (await client.PostAsJsonAsync($"/api/dues/{firstPeriodId}/items/{firstItemId}/pay-by-card", Card()))
            .EnsureSuccessStatusCode();

        // act 1 — correct the period down to 100 (resident over-paid by 400)
        var correct = await client.PutAsJsonAsync($"/api/dues/{firstPeriodId}", new { perApartmentAmount = 100m });

        // assert 1 — the over-payment shows as site credit
        correct.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var debt = await client.GetFromJsonAsync<DebtSummary>(
            $"/api/dues/sites/{siteId}/debt-summary", AuthFlow.Json);
        debt!.TotalCredit.Should().Be(400m);

        // act 2 — bill the same resident again at 100 in a new period
        var secondPeriodId = await OpenDuesPeriodAsync(client, siteId, 100m, month: 2);
        (await client.PostAsync($"/api/dues/{secondPeriodId}/distribute", content: null)).EnsureSuccessStatusCode();

        // assert 2 — the new item is settled from credit (Paid, no charge), credit drops to 300
        var secondItems = await ItemsAsync(client, secondPeriodId);
        secondItems.Should().ContainSingle().Which.Status.Should().Be("Paid");
        var debtAfter = await client.GetFromJsonAsync<DebtSummary>(
            $"/api/dues/sites/{siteId}/debt-summary", AuthFlow.Json);
        debtAfter!.TotalCredit.Should().Be(300m);
    }

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

    private static async Task<List<PeriodItem>> ItemsAsync(HttpClient client, Guid duesPeriodId)
        => (await client.GetFromJsonAsync<List<PeriodItem>>($"/api/dues/{duesPeriodId}/items", AuthFlow.Json))!;

    private static async Task<Guid> OpenDuesPeriodAsync(HttpClient client, Guid siteId, decimal amount, int month = 1)
        => await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/dues", new { siteId, year = 2026, month, perApartmentAmount = amount }));

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

    private sealed record PeriodItem(Guid ItemId, decimal Amount, string Status);

    private sealed record DebtSummary(
        Guid SiteId,
        decimal TotalAccrued,
        decimal TotalCollected,
        decimal TotalOutstanding,
        decimal TotalCredit);
}
