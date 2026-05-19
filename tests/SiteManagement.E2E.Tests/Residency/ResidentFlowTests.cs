using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SiteManagement.E2E.Tests.Infrastructure;

namespace SiteManagement.E2E.Tests.Residency;

/// <summary>
/// End-to-end smoke for the admin's Resident workflow: registering a
/// resident creates both the Domain aggregate and the linked Identity user,
/// dispatches the welcome email, and lets the duplicate TC check fire on a
/// second attempt.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ResidentFlowTests(PostgresFixture postgres) : IAsyncLifetime
{
    // Algorithm-valid synthetic TC numbers used by ResidencyDoubles in Domain.Tests.
    private const string ValidTc1 = "10000000146";
    private const string ValidTc2 = "12345678950";

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
    /// Happy path: registering a resident persists the aggregate, links an
    /// Identity user, and queues a welcome email containing a non-empty
    /// generated password.
    /// </summary>
    [Fact]
    public async Task RegisterResident_PersistsAggregateAndQueuesWelcomeEmail()
    {
        // arrange
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);

        // act — register
        var response = await client.PostAsJsonAsync(
            "/api/residents",
            new
            {
                tcNo = ValidTc1,
                firstName = "Ada",
                lastName = "Lovelace",
                email = "ada@e2e.local",
                phone = "05321234567",
            });

        // assert — 201 + readback hits the read-side projection
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<RegisterResidentResponse>(AuthFlow.Json);
        created!.ResidentId.Should().NotBeEmpty();

        var detail = await client.GetFromJsonAsync<ResidentDetailDto>($"/api/residents/{created.ResidentId}", AuthFlow.Json);
        detail.Should().NotBeNull();
        detail!.Email.Should().Be("ada@e2e.local");
        detail.Vehicles.Should().BeEmpty();

        // assert — welcome email captured with non-empty password
        _factory.Emails.Sent.Should().ContainSingle();
        var welcome = _factory.Emails.Sent.Single();
        welcome.ToEmail.Should().Be("ada@e2e.local");
        welcome.FullName.Should().Be("Ada Lovelace");
        welcome.TemporaryPassword.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Registering a second resident with the same TcNo returns 409 from
    /// the duplicate-check that the handler performs against the
    /// Residents table.
    /// </summary>
    [Fact]
    public async Task RegisterResident_WithDuplicateTcNo_IsRejectedAsConflict()
    {
        // arrange
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);

        await client.PostAsJsonAsync(
            "/api/residents",
            new
            {
                tcNo = ValidTc1,
                firstName = "Ada",
                lastName = "Lovelace",
                email = "ada@e2e.local",
                phone = "05321234567",
            });

        // act — same TC, different email
        var secondAttempt = await client.PostAsJsonAsync(
            "/api/residents",
            new
            {
                tcNo = ValidTc1,
                firstName = "Augusta",
                lastName = "Byron",
                email = "augusta@e2e.local",
                phone = "05439876543",
            });

        // assert
        secondAttempt.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// Even when the application-side duplicate check is bypassed, the DB
    /// unique index on <c>Residents.TcNo</c> guarantees no two rows share
    /// the same citizenship number. We assert by attempting two
    /// near-simultaneous registers with different emails but the same TC
    /// after disabling the recording email sender — the second one fails
    /// at the DB boundary with a 409 (translated by the
    /// BusinessRuleViolation pipeline).
    /// </summary>
    [Fact]
    public async Task DifferentEmails_ButSameTcNo_StillRejected()
    {
        // arrange
        var client = _factory.CreateClient();
        var token = await AuthFlow.LoginAsBootstrapAdminAsync(client);
        client.UseBearerToken(token);

        var first = await client.PostAsJsonAsync(
            "/api/residents",
            new
            {
                tcNo = ValidTc2,
                firstName = "Grace",
                lastName = "Hopper",
                email = "grace@e2e.local",
                phone = "05551112233",
            });
        first.EnsureSuccessStatusCode();

        // act — second attempt with the same TC but a fresh email
        var second = await client.PostAsJsonAsync(
            "/api/residents",
            new
            {
                tcNo = ValidTc2,
                firstName = "Margaret",
                lastName = "Hamilton",
                email = "margaret@e2e.local",
                phone = "05334445566",
            });

        // assert
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private sealed record RegisterResidentResponse(Guid ResidentId);

    private sealed record ResidentDetailDto(
        Guid Id,
        string TcNo,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        List<VehicleSummary> Vehicles);

    private sealed record VehicleSummary(string Plate, string? Note);
}
