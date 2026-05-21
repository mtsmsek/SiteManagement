namespace SiteManagement.Domain.Billing.Exceptions;

/// <summary>
/// Stable resource keys carried by Billing-context domain exceptions. The
/// Application layer's <c>ExceptionTranslationBehavior</c> looks each key up
/// against the <c>ErrorMessages</c> resource bundle when translating a
/// <see cref="SiteManagement.Domain.Shared.Exceptions.DomainException"/> into
/// a localized <c>BusinessRuleViolationException</c>.
/// </summary>
public static class BillingMessageKeys
{
    /// <summary><c>"Billing.BillingMonth.Invalid"</c> — month/year outside the supported range.</summary>
    public const string BillingMonthInvalid = "Billing.BillingMonth.Invalid";

    /// <summary><c>"Billing.Period.AlreadyClosed"</c> — mutating a closed period.</summary>
    public const string PeriodAlreadyClosed = "Billing.Period.AlreadyClosed";

    /// <summary><c>"Billing.Item.Duplicate"</c> — second item for the same apartment in one period.</summary>
    public const string DuplicateItem = "Billing.Item.Duplicate";

    /// <summary><c>"Billing.Item.NotFound"</c> — paying an item that isn't in the period.</summary>
    public const string ItemNotFound = "Billing.Item.NotFound";

    /// <summary><c>"Billing.Distribution.Empty"</c> — distributing with no occupied apartments.</summary>
    public const string EmptyDistribution = "Billing.Distribution.Empty";
}
