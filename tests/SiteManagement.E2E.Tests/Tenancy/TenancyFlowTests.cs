using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Tenancy;

/// <summary>
/// End-to-end for the admin's Tenancy workflow: assigning a resident occupies
/// the apartment (via the in-transaction domain event), ending the assignment
/// frees it again, the resident's history records both, and a second active
/// assignment on the same apartment is rejected by the filtered unique index.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class TenancyFlowTests(PostgresFixture postgres) : IAsyncLifetime
{
    private const string ValidTc = "10000000146";
    private const string SecondValidTc = "29876543060";

    private readonly CustomWebApplicationFactory _factory = new(postgres);

    /// <inheritdoc />
    public Task InitializeAsync() => _factory.ResetDomainDataAsync();

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Assigning a resident flips the apartment to Occupied (the assignment's
    /// domain event runs in the same transaction); ending the assignment flips
    /// it back to Empty.
    /// </summary>
    [Fact]
    public async Task Assign_OccupiesApartment_End_FreesIt()
    {
        // arrange
        var client = await CreateAdminClientAsync();
        var (siteId, apartmentId) = await SeedApartmentAsync(client);
        var residentId = await RegisterResidentAsync(client, ValidTc, "Ada", "Lovelace", "ada@e2e.local");

        // act — assign as owner
        var assignmentId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/assignments",
            new { apartmentId, residentId, tenantType = 0, startDate = "2026-01-01" }));

        // assert — apartment now occupied
        (await ReadApartmentStatusAsync(client, siteId, apartmentId)).Should().Be("Occupied");

        // act — end the assignment (move-out)
        (await client.PostAsJsonAsync($"/api/assignments/{assignmentId}/end", new { endDate = "2026-06-30" }))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        // assert — apartment free again
        (await ReadApartmentStatusAsync(client, siteId, apartmentId)).Should().Be("Empty");
    }

    /// <summary>
    /// A resident's assignment history shows the active assignment, and after
    /// move-out the same row carries the end date and is no longer active.
    /// </summary>
    [Fact]
    public async Task ResidentHistory_ReflectsAssignmentLifecycle()
    {
        // arrange
        var client = await CreateAdminClientAsync();
        var (_, apartmentId) = await SeedApartmentAsync(client);
        var residentId = await RegisterResidentAsync(client, ValidTc, "Ada", "Lovelace", "ada@e2e.local");
        var assignmentId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/assignments",
            new { apartmentId, residentId, tenantType = 1, startDate = "2026-01-01" }));

        // act — read history while active
        var active = await client.GetFromJsonAsync<List<ResidentAssignment>>(
            $"/api/assignments/residents/{residentId}", AuthFlow.Json);

        // assert
        active.Should().ContainSingle()
            .Which.Should().Match<ResidentAssignment>(a => a.IsActive && a.EndDate == null);

        // act — end, then re-read
        (await client.PostAsJsonAsync($"/api/assignments/{assignmentId}/end", new { endDate = "2026-06-30" }))
            .EnsureSuccessStatusCode();
        var ended = await client.GetFromJsonAsync<List<ResidentAssignment>>(
            $"/api/assignments/residents/{residentId}", AuthFlow.Json);

        // assert — same assignment, now closed
        ended.Should().ContainSingle()
            .Which.Should().Match<ResidentAssignment>(a => !a.IsActive && a.EndDate != null);
    }

    /// <summary>
    /// A second active assignment on an already-occupied apartment is rejected
    /// (the filtered unique index on "apartment with no end date" trips).
    /// </summary>
    [Fact]
    public async Task SecondActiveAssignment_OnSameApartment_IsRejected()
    {
        // arrange — apartment already occupied by Ada
        var client = await CreateAdminClientAsync();
        var (_, apartmentId) = await SeedApartmentAsync(client);
        var ada = await RegisterResidentAsync(client, ValidTc, "Ada", "Lovelace", "ada@e2e.local");
        (await client.PostAsJsonAsync(
            "/api/assignments",
            new { apartmentId, residentId = ada, tenantType = 0, startDate = "2026-01-01" }))
            .EnsureSuccessStatusCode();

        // act — try to assign a second resident to the same apartment, still active
        var grace = await RegisterResidentAsync(client, SecondValidTc, "Grace", "Hopper", "grace@e2e.local");
        var second = await client.PostAsJsonAsync(
            "/api/assignments",
            new { apartmentId, residentId = grace, tenantType = 1, startDate = "2026-02-01" });

        // assert — rejected as a conflict
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);
        return client;
    }

    /// <summary>Creates a site with block "A" / apartment "1"; returns (siteId, apartmentId).</summary>
    private static async Task<(Guid SiteId, Guid ApartmentId)> SeedApartmentAsync(HttpClient client)
    {
        var siteId = await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/sites", new { name = "Lavender Heights", address = "Cumhuriyet Mah. No:7" }));
        var blockId = await ReadIdAsync(await client.PostAsJsonAsync(
            $"/api/sites/{siteId}/blocks", new { name = "A" }));
        var apartmentId = await ReadIdAsync(await client.PostAsJsonAsync(
            $"/api/blocks/{blockId}/apartments", new { number = 1, floor = 1, type = "2+1" }));
        return (siteId, apartmentId);
    }

    private static async Task<Guid> RegisterResidentAsync(
        HttpClient client, string tc, string firstName, string lastName, string email)
        => await ReadIdAsync(await client.PostAsJsonAsync(
            "/api/residents",
            new { tcNo = tc, firstName, lastName, email, phone = "05321234567" }));

    /// <summary>Reads one apartment's occupancy status off the site detail projection.</summary>
    private static async Task<string> ReadApartmentStatusAsync(HttpClient client, Guid siteId, Guid apartmentId)
    {
        var site = await client.GetFromJsonAsync<SiteDetail>($"/api/sites/{siteId}", AuthFlow.Json);
        return site!.Blocks.SelectMany(b => b.Apartments).First(a => a.Id == apartmentId).Status;
    }

    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.EnumerateObject().First().Value.GetGuid();
    }

    private sealed record ResidentAssignment(
        Guid AssignmentId,
        Guid ApartmentId,
        string TenantType,
        DateOnly StartDate,
        DateOnly? EndDate,
        bool IsActive);

    private sealed record SiteDetail(Guid Id, string Name, string Address, List<BlockSummary> Blocks);
    private sealed record BlockSummary(Guid Id, string Name, List<ApartmentSummary> Apartments);
    private sealed record ApartmentSummary(Guid Id, int Number, int Floor, string Type, string Status);
}
