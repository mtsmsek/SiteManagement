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
/// End-to-end for the resident self-service portal, focused on the IDOR
/// guarantee: a resident sees and pays only their own bills. Two residents are
/// seeded; resident A then reads <c>/api/me/bills</c> (only A's lines appear),
/// pays their own item (gateway stubbed to approve), and is refused (403) when
/// trying to pay resident B's item — which stays unpaid. Role + ownership are
/// enforced by the authorization pipeline, never by the controller/handler, so
/// this proves the whole chain end to end.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ResidentPortalFlowTests(PostgresFixture postgres) : IAsyncLifetime
{
    private const string TcNoA = "10000000146";
    private const string TcNoB = "12345678950";
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

    /// <summary>A resident's own-bills view shows their line and none of another resident's.</summary>
    [Fact]
    public async Task GetMyBills_ReturnsOnlyTheCallersOwnBills()
    {
        // arrange
        var seed = await SeedTwoResidentsWithDuesAsync();
        var residentA = await LoginResidentAsync(seed.AEmail, seed.APassword);

        // act
        var bills = await residentA.GetFromJsonAsync<List<MyBill>>("/api/me/bills", AuthFlow.Json);

        // assert
        bills!.Select(b => b.ItemId).Should().ContainSingle().Which.Should().Be(seed.AItemId);
        bills!.Select(b => b.ItemId).Should().NotContain(seed.BItemId);
    }

    /// <summary>A resident can pay their own item by card; it flips to Paid.</summary>
    [Fact]
    public async Task PayMyDuesItem_OwnItem_IsAccepted_AndMarkedPaid()
    {
        // arrange — gateway approves
        StubCharge(StatusBody("Succeeded"));
        var seed = await SeedTwoResidentsWithDuesAsync();
        var residentA = await LoginResidentAsync(seed.AEmail, seed.APassword);

        // act
        var response = await residentA.PostAsJsonAsync(
            $"/api/me/dues/{seed.PeriodId}/items/{seed.AItemId}/pay-by-card", Card());

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await AdminItemStatusAsync(seed.PeriodId, seed.AItemId)).Should().Be("Paid");
    }

    /// <summary>
    /// A resident paying another resident's item is refused (403) by the
    /// ownership behavior, and that item stays Unpaid — the core IDOR guarantee.
    /// </summary>
    [Fact]
    public async Task PayMyDuesItem_AnotherResidentsItem_IsForbidden_AndLeavesItUnpaid()
    {
        // arrange — gateway would approve, but ownership must block before any charge
        StubCharge(StatusBody("Succeeded"));
        var seed = await SeedTwoResidentsWithDuesAsync();
        var residentA = await LoginResidentAsync(seed.AEmail, seed.APassword);

        // act — A targets B's item
        var response = await residentA.PostAsJsonAsync(
            $"/api/me/dues/{seed.PeriodId}/items/{seed.BItemId}/pay-by-card", Card());

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await AdminItemStatusAsync(seed.PeriodId, seed.BItemId)).Should().Be("Unpaid");
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

    /// <summary>Logs a resident in with the password from their welcome email and returns a bearer client.</summary>
    private async Task<HttpClient> LoginResidentAsync(string email, string password)
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var token = (await response.Content.ReadFromJsonAsync<TokenResponse>(AuthFlow.Json))!.AccessToken;
        client.UseBearerToken(token);
        return client;
    }

    /// <summary>Reads one dues item's status back through the admin endpoint.</summary>
    private async Task<string> AdminItemStatusAsync(Guid periodId, Guid itemId)
    {
        var admin = await CreateAdminClientAsync();
        var items = await admin.GetFromJsonAsync<List<PeriodItem>>($"/api/dues/{periodId}/items", AuthFlow.Json);
        return items!.Single(i => i.ItemId == itemId).Status;
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);
        return client;
    }

    /// <summary>
    /// Seeds a site with two occupied apartments (residents A + B), opens and
    /// distributes a dues period, and returns each resident's login + item id.
    /// </summary>
    private async Task<Seed> SeedTwoResidentsWithDuesAsync()
    {
        var admin = await CreateAdminClientAsync();

        var siteId = await ReadIdAsync(await admin.PostAsJsonAsync(
            "/api/sites", new { name = "Maple Court", address = "Bahar Mah. No:3" }));
        var blockId = await ReadIdAsync(await admin.PostAsJsonAsync(
            $"/api/sites/{siteId}/blocks", new { name = "A" }));

        var aptA = await ReadIdAsync(await admin.PostAsJsonAsync(
            $"/api/blocks/{blockId}/apartments", new { number = 1, floor = 1, type = "2+1" }));
        var aptB = await ReadIdAsync(await admin.PostAsJsonAsync(
            $"/api/blocks/{blockId}/apartments", new { number = 2, floor = 1, type = "2+1" }));

        var residentA = await ReadIdAsync(await admin.PostAsJsonAsync(
            "/api/residents",
            new { tcNo = TcNoA, firstName = "Ada", lastName = "Lovelace", email = "ada@e2e.local", phone = "05321234567" }));
        var residentB = await ReadIdAsync(await admin.PostAsJsonAsync(
            "/api/residents",
            new { tcNo = TcNoB, firstName = "Grace", lastName = "Hopper", email = "grace@e2e.local", phone = "05329876543" }));

        (await admin.PostAsJsonAsync("/api/assignments",
            new { apartmentId = aptA, residentId = residentA, tenantType = 0, startDate = "2026-01-01" }))
            .EnsureSuccessStatusCode();
        (await admin.PostAsJsonAsync("/api/assignments",
            new { apartmentId = aptB, residentId = residentB, tenantType = 0, startDate = "2026-01-01" }))
            .EnsureSuccessStatusCode();

        var periodId = await ReadIdAsync(await admin.PostAsJsonAsync(
            "/api/dues", new { siteId, year = 2026, month = 1, perApartmentAmount = 500m }));
        (await admin.PostAsync($"/api/dues/{periodId}/distribute", content: null)).EnsureSuccessStatusCode();

        var items = await admin.GetFromJsonAsync<List<PeriodItem>>($"/api/dues/{periodId}/items", AuthFlow.Json);
        var aItemId = items!.Single(i => i.ResidentId == residentA).ItemId;
        var bItemId = items!.Single(i => i.ResidentId == residentB).ItemId;

        return new Seed(
            periodId,
            "ada@e2e.local", PasswordFor("ada@e2e.local"),
            aItemId, bItemId);
    }

    /// <summary>Pulls a resident's generated password out of the recorded welcome email.</summary>
    private string PasswordFor(string email)
        => _factory.Emails.Sent.Single(e => e.ToEmail == email).TemporaryPassword;

    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.EnumerateObject().First().Value.GetGuid();
    }

    private sealed record Seed(
        Guid PeriodId,
        string AEmail,
        string APassword,
        Guid AItemId,
        Guid BItemId);

    private sealed record PeriodItem(Guid ItemId, Guid ResidentId, decimal Amount, string Status);

    private sealed record MyBill(Guid ItemId, Guid PeriodId, string Kind, decimal Amount, string Status);

    private sealed record TokenResponse(string AccessToken);
}
