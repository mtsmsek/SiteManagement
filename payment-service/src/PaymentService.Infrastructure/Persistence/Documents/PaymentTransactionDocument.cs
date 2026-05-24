namespace PaymentService.Infrastructure.Persistence.Documents;

/// <summary>
/// MongoDB persistence shape for a PaymentTransaction. Kept separate from the
/// domain aggregate so the Domain layer carries no Mongo/serialization concern;
/// the repository maps between the two. Money is flattened to amount + currency
/// and the status enum is stored as its string name for readability/stability.
/// </summary>
public sealed class PaymentTransactionDocument
{
    public Guid Id { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
}
