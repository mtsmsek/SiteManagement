namespace SiteManagement.Domain.Tenancy.Exceptions;

/// <summary>
/// Stable resource keys carried by Tenancy-context domain exceptions. The
/// Application layer's <c>ExceptionTranslationBehavior</c> looks each key up
/// against the <c>ErrorMessages</c> resource bundle when translating a
/// <see cref="SiteManagement.Domain.Shared.Exceptions.DomainException"/> into
/// a localized <c>BusinessRuleViolationException</c>.
/// </summary>
public static class TenancyMessageKeys
{
    /// <summary><c>"Tenancy.AssignmentPeriod.Invalid"</c> — end date precedes start.</summary>
    public const string AssignmentPeriodInvalid = "Tenancy.AssignmentPeriod.Invalid";

    /// <summary><c>"Tenancy.Assignment.AlreadyEnded"</c> — closing an already-ended assignment.</summary>
    public const string AssignmentAlreadyEnded = "Tenancy.Assignment.AlreadyEnded";
}
