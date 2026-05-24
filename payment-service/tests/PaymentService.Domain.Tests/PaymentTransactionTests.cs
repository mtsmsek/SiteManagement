using FluentAssertions;
using PaymentService.Domain;
using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared.ValueObjects;

namespace PaymentService.Domain.Tests;

/// <summary>
/// Specifies the <see cref="PaymentTransaction"/> aggregate: it records one
/// charge attempt, keyed by an idempotency key, and settles exactly once to
/// Succeeded or Failed. Re-settling a finished transaction is rejected.
/// </summary>
public class PaymentTransactionTests
{
    private const string IdemKey = "dues-item-123-attempt-1";
    private const string Reference = "billing-item:123";

    private static PaymentTransaction Start()
        => PaymentTransaction.Start(IdemKey, Reference, Money.Of(500m));

    [Fact]
    public void Start_IsPending_WithKeyReferenceAndAmount()
    {
        // act
        var tx = Start();

        // assert
        tx.Id.Should().NotBeEmpty();
        tx.IdempotencyKey.Should().Be(IdemKey);
        tx.Reference.Should().Be(Reference);
        tx.Amount.Should().Be(Money.Of(500m));
        tx.Status.Should().Be(PaymentStatus.Pending);
        tx.FailureReason.Should().BeNull();
    }

    [Fact]
    public void Succeed_MovesToSucceeded()
    {
        // arrange
        var tx = Start();

        // act
        tx.Succeed();

        // assert
        tx.Status.Should().Be(PaymentStatus.Succeeded);
        tx.FailureReason.Should().BeNull();
    }

    [Fact]
    public void Fail_MovesToFailed_WithReason()
    {
        // arrange
        var tx = Start();

        // act
        tx.Fail("insufficient_balance");

        // assert
        tx.Status.Should().Be(PaymentStatus.Failed);
        tx.FailureReason.Should().Be("insufficient_balance");
    }

    [Fact]
    public void Succeed_AfterAlreadySettled_Throws()
    {
        // arrange
        var tx = Start();
        tx.Succeed();

        // act
        var act = () => tx.Succeed();

        // assert
        act.Should().Throw<InvalidTransactionStateException>();
    }

    [Fact]
    public void Fail_AfterAlreadySettled_Throws()
    {
        // arrange
        var tx = Start();
        tx.Fail("rejected");

        // act
        var act = () => tx.Succeed();

        // assert
        act.Should().Throw<InvalidTransactionStateException>();
    }
}
