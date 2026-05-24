using PaymentService.Application.Abstractions;
using PaymentService.Domain;
using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared.ValueObjects;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Payments;

/// <summary>
/// Processes a card charge against the fake bank. Idempotent by the request's
/// key. Declines (insufficient funds, expired/unknown card) are recorded as
/// Failed transactions rather than thrown, so every attempt is auditable and
/// the caller gets a uniform result; only unexpected errors bubble up.
/// </summary>
public sealed class PaymentProcessor(
    IBankAccountRepository accounts,
    ICreditCardRepository cards,
    IPaymentTransactionRepository transactions,
    TimeProvider clock) : IPaymentProcessor
{
    private readonly IBankAccountRepository _accounts = accounts;
    private readonly ICreditCardRepository _cards = cards;
    private readonly IPaymentTransactionRepository _transactions = transactions;
    private readonly TimeProvider _clock = clock;

    /// <inheritdoc />
    public async Task<ProcessPaymentResult> ProcessAsync(ProcessPaymentRequest request, CancellationToken ct = default)
    {
        // Idempotency: a prior attempt under this key wins — return its result,
        // charge nothing again.
        var existing = await _transactions.FindByIdempotencyKeyAsync(request.IdempotencyKey, ct);
        if (existing is not null)
        {
            return ToResult(existing);
        }

        var amount = Money.Of(request.Amount);
        var transaction = PaymentTransaction.Start(request.IdempotencyKey, request.Reference, amount);

        try
        {
            var card = await _cards.FindByNumberAsync(CardNumber.From(request.CardNumber), ct)
                ?? throw new CardRejectedException("unknown_card");

            card.EnsureUsable(DateOnly.FromDateTime(_clock.GetUtcNow().UtcDateTime));

            var account = await _accounts.GetByIdAsync(card.BankAccountId, ct)
                ?? throw new CardRejectedException("no_account");

            account.Debit(amount); // throws InsufficientBalanceException when short
            await _accounts.UpdateAsync(account, ct);

            transaction.Succeed();
        }
        catch (Exception ex) when (ex is InsufficientBalanceException or CardRejectedException or InvalidCardException)
        {
            transaction.Fail(ReasonFor(ex));
        }

        await _transactions.AddAsync(transaction, ct);
        return ToResult(transaction);
    }

    private static string ReasonFor(Exception ex) => ex switch
    {
        InsufficientBalanceException => "insufficient_balance",
        InvalidCardException => "invalid_card",
        CardRejectedException rejected => rejected.MessageArgs.Count > 0 ? rejected.MessageArgs[0].ToString()! : "rejected",
        _ => "rejected",
    };

    private static ProcessPaymentResult ToResult(PaymentTransaction tx)
        => new(tx.Id, tx.Status.ToString(), tx.FailureReason);
}
