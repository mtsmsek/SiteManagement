namespace SiteManagement.Domain.Billing;

/// <summary>
/// Size limits for the Billing bounded context. Persistence-facing constants
/// (column widths, monetary precision) live here next to the domain types so
/// the EF configuration references a named limit rather than a magic number —
/// mirroring <c>PropertyLimits</c>.
/// </summary>
public static class BillingLimits
{
    /// <summary>Max stored length of <see cref="UtilityType"/> / <see cref="BillingItemStatus"/> as a string.</summary>
    public const int EnumNameMaxLength = 20;

    /// <summary>Max stored length of a <see cref="ValueObjects.BillingMonth"/> string ("yyyy-MM").</summary>
    public const int BillingMonthLength = 7;

    /// <summary>Total decimal digits stored for a monetary amount.</summary>
    public const int MoneyPrecision = 18;

    /// <summary>Decimal places stored for a monetary amount (kuruş).</summary>
    public const int MoneyScale = 2;
}
