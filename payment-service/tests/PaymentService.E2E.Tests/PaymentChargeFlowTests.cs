using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Abstractions;
using PaymentService.E2E.Tests.Infrastructure;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.E2E.Tests;

/// <summary>
/// End-to-end for the PaymentService charge pipeline over real HTTP against a
/// real Mongo container: a charge runs controller → processor → fake bank →
/// Mongo, declines come back as a 200 with a Failed status (a business outcome,
/// not a transport error), and the idempotency key collapses repeats to a
/// single transaction.
/// </summary>
[Collection(PaymentApiCollection.Name)]
public sealed class PaymentChargeFlowTests(MongoFixture mongo) : IAsyncLifetime
{
    private readonly PaymentApiFactory _factory = new(mongo);

    /// <inheritdoc />
    public Task InitializeAsync() => Task.CompletedTask;

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Charging the seeded demo card for an affordable amount succeeds: the
    /// endpoint returns a Succeeded status and the transaction is persisted in
    /// Mongo under the request's idempotency key.
    /// </summary>
    [Fact]
    public async Task Charge_WithFundedCard_SucceedsAndPersistsTransaction()
    {
        // arrange
        var client = _factory.CreateClient();
        var key = $"e2e-success:{Guid.NewGuid():N}";
        var request = ChargeBody(key, PaymentSeeder.DemoCardNumber, amount: 500m);

        // act
        var response = await client.PostAsJsonAsync("/api/payments", request);
        var result = await response.Content.ReadFromJsonAsync<ChargeResult>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Status.Should().Be("Succeeded");
        result.FailureReason.Should().BeNull();
        var persisted = await FindTransactionAsync(key);
        persisted.Should().NotBeNull();
        persisted!.Status.ToString().Should().Be("Succeeded");
    }

    /// <summary>
    /// Charging more than the funded account holds is declined: a 200 carrying a
    /// Failed status with the <c>insufficient_balance</c> reason.
    /// </summary>
    [Fact]
    public async Task Charge_BeyondAccountBalance_IsDeclinedAsFailed()
    {
        // arrange — the demo account opens with 100_000; ask for far more
        var client = _factory.CreateClient();
        var key = $"e2e-insufficient:{Guid.NewGuid():N}";
        var request = ChargeBody(key, PaymentSeeder.DemoCardNumber, amount: 1_000_000m);

        // act
        var response = await client.PostAsJsonAsync("/api/payments", request);
        var result = await response.Content.ReadFromJsonAsync<ChargeResult>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Status.Should().Be("Failed");
        result.FailureReason.Should().Be("insufficient_balance");
    }

    /// <summary>
    /// Charging a card the gateway has never issued is declined as Failed with
    /// the <c>unknown_card</c> reason rather than blowing up.
    /// </summary>
    [Fact]
    public async Task Charge_WithUnknownCard_IsDeclinedAsFailed()
    {
        // arrange — a Luhn-valid PAN that was never seeded
        var client = _factory.CreateClient();
        var key = $"e2e-unknown:{Guid.NewGuid():N}";
        var request = ChargeBody(key, cardNumber: "4111111111111111", amount: 100m);

        // act
        var response = await client.PostAsJsonAsync("/api/payments", request);
        var result = await response.Content.ReadFromJsonAsync<ChargeResult>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Status.Should().Be("Failed");
        result.FailureReason.Should().Be("unknown_card");
    }

    /// <summary>
    /// Posting the same idempotency key twice charges once: the second call
    /// returns the first transaction's id and status, and Mongo holds a single
    /// transaction for that key.
    /// </summary>
    [Fact]
    public async Task Charge_RepeatedWithSameKey_ReturnsFirstTransactionOnce()
    {
        // arrange
        var client = _factory.CreateClient();
        var key = $"e2e-idempotent:{Guid.NewGuid():N}";
        var request = ChargeBody(key, PaymentSeeder.DemoCardNumber, amount: 250m);

        // act — charge twice under the same key
        var first = await (await client.PostAsJsonAsync("/api/payments", request))
            .Content.ReadFromJsonAsync<ChargeResult>();
        var second = await (await client.PostAsJsonAsync("/api/payments", request))
            .Content.ReadFromJsonAsync<ChargeResult>();

        // assert — same settled transaction returned both times
        second!.TransactionId.Should().Be(first!.TransactionId);
        second.Status.Should().Be("Succeeded");
        var persisted = await FindTransactionAsync(key);
        persisted!.Id.Should().Be(first.TransactionId);
    }

    private static object ChargeBody(string idempotencyKey, string cardNumber, decimal amount) => new
    {
        idempotencyKey,
        cardNumber,
        cvv = "123",
        expiryYear = 2030,
        expiryMonth = 12,
        amount,
        reference = idempotencyKey,
    };

    /// <summary>Reads back the transaction stored under a key, straight from Mongo.</summary>
    private async Task<Domain.PaymentTransaction?> FindTransactionAsync(string idempotencyKey)
    {
        using var scope = _factory.Services.CreateScope();
        var transactions = scope.ServiceProvider.GetRequiredService<IPaymentTransactionRepository>();
        return await transactions.FindByIdempotencyKeyAsync(idempotencyKey);
    }

    private sealed record ChargeResult(Guid TransactionId, string Status, string? FailureReason);
}
