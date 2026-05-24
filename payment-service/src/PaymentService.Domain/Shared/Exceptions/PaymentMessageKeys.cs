namespace PaymentService.Domain.Shared.Exceptions;

/// <summary>
/// Stable resource keys carried by PaymentService domain exceptions. The
/// Application layer maps these to localized HTTP problem responses.
/// </summary>
public static class PaymentMessageKeys
{
    /// <summary><c>"Payment.Money.Negative"</c> — a monetary amount went below zero.</summary>
    public const string MoneyNegative = "Payment.Money.Negative";

    /// <summary><c>"Payment.Card.Invalid"</c> — card number/cvv/expiry failed validation.</summary>
    public const string CardInvalid = "Payment.Card.Invalid";

    /// <summary><c>"Payment.Account.InsufficientBalance"</c> — debit exceeds the balance.</summary>
    public const string InsufficientBalance = "Payment.Account.InsufficientBalance";

    /// <summary><c>"Payment.Card.Rejected"</c> — card declined (expired / failed check).</summary>
    public const string CardRejected = "Payment.Card.Rejected";

    /// <summary><c>"Payment.Transaction.InvalidState"</c> — illegal transaction state transition.</summary>
    public const string TransactionInvalidState = "Payment.Transaction.InvalidState";
}
