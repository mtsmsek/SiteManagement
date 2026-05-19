using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Property;

/// <summary>
/// End-to-end smoke for the admin's Property workflow: create a site,
/// add a block, add an apartment, mark it occupied, and read the full
/// hierarchy back via the detail endpoint.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class PropertyFlowTests(PostgresFixture postgres) : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory = new(postgres);

    /// <inheritdoc />
    public async Task InitializeAsync() => await _factory.ResetDomainDataAsync();

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>Full admin journey: site → block → apartment → occupy → detail readback.</summary>
    [Fact]
    public async Task Admin_CanBuildFullPropertyHierarchy()
    {
        // arrange
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);

        // act + assert — create site
        var siteResponse = await client.PostAsJsonAsync(
            "/api/sites",
            new { name = "Lavender Heights", address = "Cumhuriyet Mah. No:7" });
        siteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var siteId = (await siteResponse.Content.ReadFromJsonAsync<CreateSiteResponse>(AuthFlow.Json))!.SiteId;

        // act + assert — add block
        var blockResponse = await client.PostAsJsonAsync(
            $"/api/sites/{siteId}/blocks",
            new { name = "A" });
        blockResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var blockId = (await blockResponse.Content.ReadFromJsonAsync<AddBlockResponse>(AuthFlow.Json))!.BlockId;

        // act + assert — add apartment
        var apartmentResponse = await client.PostAsJsonAsync(
            $"/api/blocks/{blockId}/apartments",
            new { number = 1, floor = 1, type = "2+1" });
        apartmentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var apartmentId = (await apartmentResponse.Content.ReadFromJsonAsync<AddApartmentResponse>(AuthFlow.Json))!.ApartmentId;

        // act + assert — occupy
        var occupyResponse = await client.PostAsync($"/api/apartments/{apartmentId}/occupy", content: null);
        occupyResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // act — readback
        var detail = await client.GetFromJsonAsync<SiteDetailDto>($"/api/sites/{siteId}", AuthFlow.Json);

        // assert — full hierarchy materialised, status flipped
        detail.Should().NotBeNull();
        detail!.Blocks.Should().ContainSingle();
        detail.Blocks[0].Apartments.Should().ContainSingle()
            .Which.Status.Should().Be("Occupied");
    }

    /// <summary>Duplicate block names within one site are rejected with 409.</summary>
    [Fact]
    public async Task AddingBlockWithDuplicateName_IsRejectedAsConflict()
    {
        // arrange
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);

        var siteResponse = await client.PostAsJsonAsync(
            "/api/sites",
            new { name = "Sunset Park", address = "Address" });
        var siteId = (await siteResponse.Content.ReadFromJsonAsync<CreateSiteResponse>(AuthFlow.Json))!.SiteId;

        var firstAdd = await client.PostAsJsonAsync($"/api/sites/{siteId}/blocks", new { name = "A" });
        firstAdd.EnsureSuccessStatusCode();

        // act — adding "a" (case-insensitive duplicate) must fail
        var secondAdd = await client.PostAsJsonAsync($"/api/sites/{siteId}/blocks", new { name = "a" });

        // assert
        secondAdd.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private sealed record CreateSiteResponse(Guid SiteId);
    private sealed record AddBlockResponse(Guid BlockId);
    private sealed record AddApartmentResponse(Guid ApartmentId);

    private sealed record SiteDetailDto(
        Guid Id,
        string Name,
        string Address,
        List<BlockSummary> Blocks);

    private sealed record BlockSummary(Guid Id, string Name, List<ApartmentSummary> Apartments);
    private sealed record ApartmentSummary(Guid Id, int Number, int Floor, string Type, string Status);
}
