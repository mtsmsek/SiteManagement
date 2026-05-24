using FluentAssertions;
using NSubstitute;
using PaymentService.Application.Abstractions;
using PaymentService.Application.Payments;
using PaymentService.Domain;
using PaymentService.Domain.Shared.ValueObjects;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Tests.Application;

/// <summary>
/// Unit tests for <see cref="PaymentProcessor"/>: idempotency (a repeated key
/// returns the prior result without charging), happy path (debit + Succeeded),
/// and declines (insufficient funds, expired card, unknown card) — each
/// recorded as a Failed transaction so the outcome is auditable.
/// </summary>
public class PaymentProcessorTests
{
    private const string ValidPan = "4242424242424242";
    private const string IdemKey = "item-1-attempt-1";

    private readonly IBankAccountRepository _accounts = Substitute.For<IBankAccountRepository>();
    private readonly ICreditCardRepository _cards = Substitute.For<ICreditCardRepository>();
    private readonly IPaymentTransactionRepository _transactions = Substitute.For<IPaymentTransactionRepository>();
    private readonly TimeProvider _clock = Substitute.For<TimeProvider>();

    public PaymentProcessorTests()
        => _clock.GetUtcNow().Returns(new DateTimeOffset(2026, 5, 25, 0, 0, 0, TimeSpan.Zero));

    private PaymentProcessor CreateSut() => new(_accounts, _cards, _transactions, _clock);

    private static ProcessPaymentRequest Request(decimal amount = 200m)
        => new(IdemKey, ValidPan, "123", 2030, 12, amount, "billing-item:1");

    private (BankAccount Account, CreditCard Card) SeedUsableCard(decimal balance)
    {
        var account = BankAccount.Open(Money.Of(balance));
        var card = CreditCard.Issue(account.Id, CardNumber.From(ValidPan), Cvv.From("123"), ExpiryDate.Of(2030, 12));
        _cards.FindByNumberAsync(Arg.Any<CardNumber>(), Arg.Any<CancellationToken>()).Returns(card);
        _accounts.GetByIdAsync(account.Id, Arg.Any<CancellationToken>()).Returns(account);
        return (account, card);
    }

    [Fact]
    public async Task Process_RepeatedKey_ReturnsPriorResult_WithoutCharging()
    {
        // arrange — a transaction already exists for this key
        var existing = PaymentTransaction.Start(IdemKey, "billing-item:1", Money.Of(200m));
        existing.Succeed();
        _transactions.FindByIdempotencyKeyAsync(IdemKey, Arg.Any<CancellationToken>()).Returns(existing);
        var sut = CreateSut();

        // act
        var result = await sut.ProcessAsync(Request());

        // assert — prior result returned, nothing new charged or stored
        result.TransactionId.Should().Be(existing.Id);
        result.Status.Should().Be("Succeeded");
        await _accounts.DidNotReceive().UpdateAsync(Arg.Any<BankAccount>(), Arg.Any<CancellationToken>());
        await _transactions.DidNotReceive().AddAsync(Arg.Any<PaymentTransaction>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Process_HappyPath_DebitsAndSucceeds()
    {
        // arrange
        var (account, _) = SeedUsableCard(balance: 500m);
        var sut = CreateSut();

        // act
        var result = await sut.ProcessAsync(Request(amount: 200m));

        // assert — balance debited, transaction succeeded + stored
        result.Status.Should().Be("Succeeded");
        account.Balance.Should().Be(Money.Of(300m));
        await _accounts.Received(1).UpdateAsync(account, Arg.Any<CancellationToken>());
        await _transactions.Received(1).AddAsync(
            Arg.Is<PaymentTransaction>(t => t.Status == PaymentStatus.Succeeded), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Process_InsufficientBalance_FailsAndDoesNotDebit()
    {
        // arrange — balance below the charge
        var (account, _) = SeedUsableCard(balance: 100m);
        var sut = CreateSut();

        // act
        var result = await sut.ProcessAsync(Request(amount: 200m));

        // assert — failed, balance untouched, a Failed transaction recorded
        result.Status.Should().Be("Failed");
        account.Balance.Should().Be(Money.Of(100m));
        await _accounts.DidNotReceive().UpdateAsync(Arg.Any<BankAccount>(), Arg.Any<CancellationToken>());
        await _transactions.Received(1).AddAsync(
            Arg.Is<PaymentTransaction>(t => t.Status == PaymentStatus.Failed), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Process_ExpiredCard_Fails()
    {
        // arrange — card expired before the clock's "now"
        var account = BankAccount.Open(Money.Of(500m));
        var card = CreditCard.Issue(account.Id, CardNumber.From(ValidPan), Cvv.From("123"), ExpiryDate.Of(2024, 1));
        _cards.FindByNumberAsync(Arg.Any<CardNumber>(), Arg.Any<CancellationToken>()).Returns(card);
        _accounts.GetByIdAsync(account.Id, Arg.Any<CancellationToken>()).Returns(account);
        var sut = CreateSut();

        // act
        var result = await sut.ProcessAsync(Request());

        // assert
        result.Status.Should().Be("Failed");
        await _transactions.Received(1).AddAsync(
            Arg.Is<PaymentTransaction>(t => t.Status == PaymentStatus.Failed), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Process_UnknownCard_Fails()
    {
        // arrange — no card on file
        _cards.FindByNumberAsync(Arg.Any<CardNumber>(), Arg.Any<CancellationToken>()).Returns((CreditCard?)null);
        var sut = CreateSut();

        // act
        var result = await sut.ProcessAsync(Request());

        // assert
        result.Status.Should().Be("Failed");
        await _transactions.Received(1).AddAsync(
            Arg.Is<PaymentTransaction>(t => t.Status == PaymentStatus.Failed), Arg.Any<CancellationToken>());
    }
}
