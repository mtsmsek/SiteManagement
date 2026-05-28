using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Messaging;

/// <summary>
/// End-to-end for admin ↔ resident messaging. An admin opens a thread with a
/// resident; the resident sees it in their own inbox (with an unread badge),
/// reads it, and replies — which the admin then sees. The IDOR guarantee is
/// covered too: a resident cannot read another resident's conversation.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class MessagingFlowTests(PostgresFixture postgres) : IAsyncLifetime
{
    private const string TcNoA = "10000000146";
    private const string TcNoB = "12345678950";
    private const string EmailA = "ada@e2e.local";
    private const string EmailB = "grace@e2e.local";

    private readonly PostgresFixture _postgres = postgres;
    private CustomWebApplicationFactory _factory = null!;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory(_postgres);
        await _factory.ResetDomainDataAsync();
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>Admin opens a thread; the resident sees it unread, reads, and replies; the admin sees the reply.</summary>
    [Fact]
    public async Task AdminOpensThread_ResidentSeesReadsAndReplies()
    {
        // arrange
        var admin = await CreateAdminClientAsync();
        var residentAId = await RegisterResidentAsync(admin, TcNoA, "Ada", "Lovelace", EmailA);
        var conversationId = await StartConversationAsync(admin, residentAId, "Aidat hatırlatması", "Merhaba, aidat borcunuz var.");
        var residentA = await LoginResidentAsync(EmailA);

        // act — resident lists their inbox
        var inbox = await residentA.GetFromJsonAsync<List<ConversationRow>>("/api/me/conversations", AuthFlow.Json);

        // assert — the thread is there with one unread (the admin's opener)
        var row = inbox!.Single(c => c.Id == conversationId);
        row.UnreadForResident.Should().Be(1);

        // assert — admin inbox carries the resident's display name for the row
        var adminInboxFirst = await admin.GetFromJsonAsync<List<ConversationRow>>("/api/conversations", AuthFlow.Json);
        adminInboxFirst!.Single(c => c.Id == conversationId).ResidentName.Should().Be("Ada Lovelace");

        // act — resident reads + replies
        (await residentA.PostAsync($"/api/me/conversations/{conversationId}/read", content: null)).EnsureSuccessStatusCode();
        (await residentA.PostAsJsonAsync($"/api/me/conversations/{conversationId}/messages", new { body = "Teşekkürler, hallediyorum." }))
            .EnsureSuccessStatusCode();

        // assert — admin sees both messages; the resident's reply is unread for the admin
        var messages = await admin.GetFromJsonAsync<List<MessageRow>>($"/api/conversations/{conversationId}/messages", AuthFlow.Json);
        messages!.Should().HaveCount(2);
        messages!.Select(m => m.SenderRole).Should().ContainInOrder("Admin", "Resident");

        var adminInbox = await admin.GetFromJsonAsync<List<ConversationRow>>("/api/conversations", AuthFlow.Json);
        adminInbox!.Single(c => c.Id == conversationId).UnreadForAdmin.Should().Be(1);
    }

    /// <summary>A resident cannot read another resident's conversation (403).</summary>
    [Fact]
    public async Task ResidentCannotReadAnotherResidentsConversation()
    {
        // arrange — two residents, each with their own admin-opened thread
        var admin = await CreateAdminClientAsync();
        var residentAId = await RegisterResidentAsync(admin, TcNoA, "Ada", "Lovelace", EmailA);
        var residentBId = await RegisterResidentAsync(admin, TcNoB, "Grace", "Hopper", EmailB);
        await StartConversationAsync(admin, residentAId, "A konusu", "A için mesaj");
        var bConversationId = await StartConversationAsync(admin, residentBId, "B konusu", "B için mesaj");
        var residentA = await LoginResidentAsync(EmailA);

        // act — A tries to read B's conversation
        var response = await residentA.GetAsync($"/api/me/conversations/{bConversationId}/messages");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);
        return client;
    }

    private async Task<HttpClient> LoginResidentAsync(string email)
    {
        var password = _factory.Emails.Sent.Single(e => e.ToEmail == email).TemporaryPassword;
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var token = (await response.Content.ReadFromJsonAsync<TokenResponse>(AuthFlow.Json))!.AccessToken;
        client.UseBearerToken(token);
        return client;
    }

    private static async Task<Guid> RegisterResidentAsync(
        HttpClient admin, string tcNo, string firstName, string lastName, string email)
        => await ReadIdAsync(await admin.PostAsJsonAsync(
            "/api/residents",
            new { tcNo, firstName, lastName, email, phone = "05321234567" }));

    private static async Task<Guid> StartConversationAsync(HttpClient admin, Guid residentId, string subject, string body)
        => await ReadIdAsync(await admin.PostAsJsonAsync(
            "/api/conversations", new { residentId, subject, body }));

    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.EnumerateObject().First().Value.GetGuid();
    }

    private sealed record ConversationRow(Guid Id, string ResidentName, int UnreadForAdmin, int UnreadForResident);

    private sealed record MessageRow(Guid Id, string SenderRole, string Body);

    private sealed record TokenResponse(string AccessToken);
}
