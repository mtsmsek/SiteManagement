using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Messaging;

/// <summary>
/// End-to-end for the SignalR <c>MessagingHub</c>: with both an admin and a
/// resident connected via real HubConnections (over the TestServer with
/// LongPolling, which it supports — WebSockets are not available in the
/// in-memory host), an admin opening a thread pushes
/// <c>ConversationStarted</c> to the resident, and a resident reply pushes
/// <c>MessageReceived</c> to the admin. Together they prove that the
/// notifier wiring (group + JWT-on-query-string + role-based group join)
/// is end-to-end correct.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class SignalRMessagingTests(PostgresFixture postgres) : IAsyncLifetime
{
    private const string TcNo = "10000000146";
    private const string Email = "ada@e2e.local";

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

    [Fact]
    public async Task AdminStartsConversation_PushesConversationStartedToResident()
    {
        // arrange
        var admin = _factory.CreateClient();
        var adminToken = await AuthFlow.LoginAsBootstrapAdminAsync(admin);
        admin.UseBearerToken(adminToken);

        var residentId = await RegisterResidentAsync(admin);
        var residentToken = await LoginResidentTokenAsync(Email);

        await using var residentHub = await ConnectHubAsync(residentToken);
        var received = WaitFor<ConversationStartedPayload>(residentHub, "ConversationStarted");

        // act — admin opens a thread; the resident should be pushed a "ConversationStarted"
        var response = await admin.PostAsJsonAsync(
            "/api/conversations", new { residentId, subject = "S", body = "B" });
        response.EnsureSuccessStatusCode();

        // assert
        var payload = await received.WaitAsync(TimeSpan.FromSeconds(5));
        payload.ResidentId.Should().Be(residentId);
    }

    [Fact]
    public async Task ResidentReplies_PushesMessageReceivedToAdmin()
    {
        // arrange
        var admin = _factory.CreateClient();
        var adminToken = await AuthFlow.LoginAsBootstrapAdminAsync(admin);
        admin.UseBearerToken(adminToken);
        var residentId = await RegisterResidentAsync(admin);

        // admin opens a thread so the resident has one to reply to
        var startResponse = await admin.PostAsJsonAsync(
            "/api/conversations", new { residentId, subject = "S", body = "B" });
        startResponse.EnsureSuccessStatusCode();
        var conversationId = (await startResponse.Content.ReadFromJsonAsync<ConversationCreated>(AuthFlow.Json))!.ConversationId;

        var residentClient = _factory.CreateClient();
        var residentToken = await LoginResidentTokenAsync(Email);
        residentClient.UseBearerToken(residentToken);

        await using var adminHub = await ConnectHubAsync(adminToken);
        var received = WaitFor<MessageReceivedPayload>(adminHub, "MessageReceived");

        // act — resident replies; admin should be pushed a "MessageReceived"
        var replyResponse = await residentClient.PostAsJsonAsync(
            $"/api/me/conversations/{conversationId}/messages", new { body = "Yanıt" });
        replyResponse.EnsureSuccessStatusCode();

        // assert
        var payload = await received.WaitAsync(TimeSpan.FromSeconds(5));
        payload.ConversationId.Should().Be(conversationId);
    }

    private async Task<HubConnection> ConnectHubAsync(string accessToken)
    {
        var server = _factory.Server;
        var url = $"{server.BaseAddress}hubs/messaging?access_token={accessToken}";
        var connection = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.HttpMessageHandlerFactory = _ => server.CreateHandler();
                options.Transports = HttpTransportType.LongPolling;
            })
            .Build();
        await connection.StartAsync();
        return connection;
    }

    private static Task<T> WaitFor<T>(HubConnection connection, string eventName)
    {
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.On<T>(eventName, payload => tcs.TrySetResult(payload));
        return tcs.Task;
    }

    private async Task<Guid> RegisterResidentAsync(HttpClient admin)
    {
        var response = await admin.PostAsJsonAsync(
            "/api/residents", new { tcNo = TcNo, firstName = "Ada", lastName = "Lovelace", email = Email, phone = "05321234567" });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<RegisterResidentResponse>(AuthFlow.Json);
        return body!.ResidentId;
    }

    private async Task<string> LoginResidentTokenAsync(string email)
    {
        var password = _factory.Emails.Sent.Single(e => e.ToEmail == email).TemporaryPassword;
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var token = (await response.Content.ReadFromJsonAsync<TokenResponse>(AuthFlow.Json))!.AccessToken;
        return token;
    }

    private sealed record ConversationStartedPayload(Guid ConversationId, Guid ResidentId);

    private sealed record MessageReceivedPayload(Guid ConversationId);

    private sealed record ConversationCreated(Guid ConversationId);

    private sealed record RegisterResidentResponse(Guid ResidentId);

    private sealed record TokenResponse(string AccessToken);
}
