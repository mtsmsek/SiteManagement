using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Domain.Property;
using SiteManagement.E2E.Tests.Infrastructure;
using SiteManagement.Infrastructure.Persistence;

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

        // assert — conflict, and the localized detail has the block name
        // substituted (no leaked "{0}" placeholder). Guards the bug where the
        // middleware re-localized the bare key, dropping the format args.
        secondAdd.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await secondAdd.Content.ReadFromJsonAsync<ProblemResponse>(AuthFlow.Json);
        problem!.Detail.Should().NotContain("{0}");
        problem.Detail.Should().Contain("a");
    }

    /// <summary>
    /// The trash-can delete is a soft delete: the site vanishes from reads but
    /// its row (and its blocks/apartments) stay in the database, flagged.
    /// </summary>
    [Fact]
    public async Task DeleteSite_ArchivesIt_HiddenFromReadsButKept()
    {
        // arrange
        var client = await CreateAdminClientAsync();
        var siteId = await BuildSiteWithApartmentAsync(client);

        // act
        var delete = await client.DeleteAsync($"/api/sites/{siteId}");

        // assert — gone from the list, but still in the DB with the flag set
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var list = await client.GetFromJsonAsync<List<SiteRow>>("/api/sites", AuthFlow.Json);
        list!.Should().NotContain(s => s.Id == siteId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var archived = await db.Sites.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == siteId);
        archived.Should().NotBeNull();
        archived!.IsDeleted.Should().BeTrue();
        archived.DeletedOnUtc.Should().NotBeNull();
        (await db.Set<Apartment>().IgnoreQueryFilters().CountAsync()).Should().Be(1);
    }

    /// <summary>
    /// The permanent purge hard-deletes an (already archived) site and cascades
    /// to its blocks + apartments — the rows are gone for good.
    /// </summary>
    [Fact]
    public async Task PurgeSite_HardDeletes_RemovingTheRowsForGood()
    {
        // arrange — build then archive a site
        var client = await CreateAdminClientAsync();
        var siteId = await BuildSiteWithApartmentAsync(client);
        (await client.DeleteAsync($"/api/sites/{siteId}")).EnsureSuccessStatusCode();

        // act — purge the archived site
        var purge = await client.DeleteAsync($"/api/sites/{siteId}/permanent");

        // assert — nothing left, even ignoring the soft-delete filter
        purge.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        (await db.Sites.IgnoreQueryFilters().AnyAsync(s => s.Id == siteId)).Should().BeFalse();
        (await db.Set<Block>().CountAsync()).Should().Be(0);
        (await db.Set<Apartment>().CountAsync()).Should().Be(0);
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);
        return client;
    }

    private static async Task<Guid> BuildSiteWithApartmentAsync(HttpClient client)
    {
        var siteResponse = await client.PostAsJsonAsync(
            "/api/sites", new { name = "Soft Delete Demo", address = "Addr" });
        var siteId = (await siteResponse.Content.ReadFromJsonAsync<CreateSiteResponse>(AuthFlow.Json))!.SiteId;

        var blockResponse = await client.PostAsJsonAsync($"/api/sites/{siteId}/blocks", new { name = "A" });
        var blockId = (await blockResponse.Content.ReadFromJsonAsync<AddBlockResponse>(AuthFlow.Json))!.BlockId;

        await client.PostAsJsonAsync(
            $"/api/blocks/{blockId}/apartments", new { number = 1, floor = 1, type = "2+1" });

        return siteId;
    }

    private sealed record SiteRow(Guid Id);

    private sealed record CreateSiteResponse(Guid SiteId);
    private sealed record AddBlockResponse(Guid BlockId);
    private sealed record AddApartmentResponse(Guid ApartmentId);
    private sealed record ProblemResponse(string? Detail, string? MessageKey);

    private sealed record SiteDetailDto(
        Guid Id,
        string Name,
        string Address,
        List<BlockSummary> Blocks);

    private sealed record BlockSummary(Guid Id, string Name, List<ApartmentSummary> Apartments);
    private sealed record ApartmentSummary(Guid Id, int Number, int Floor, string Type, string Status);
}
