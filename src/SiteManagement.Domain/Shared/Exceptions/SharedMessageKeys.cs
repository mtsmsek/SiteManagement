namespace SiteManagement.Domain.Shared.Exceptions;

/// <summary>
/// Stable resource keys carried by cross-context (Shared kernel) domain
/// exceptions — those not owned by a single bounded context, such as the
/// <see cref="SiteManagement.Domain.Shared.ValueObjects.Money"/> value object.
/// The Application layer's <c>ExceptionTranslationBehavior</c> looks each key
/// up against the <c>ErrorMessages</c> resource bundle.
/// </summary>
public static class SharedMessageKeys
{
    /// <summary><c>"Shared.Money.Negative"</c> — a monetary amount went below zero.</summary>
    public const string MoneyNegative = "Shared.Money.Negative";
}
