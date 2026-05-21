namespace SiteManagement.Domain.Billing;

/// <summary>
/// The utility a <see cref="UtilityBillPeriod"/> distributes the cost of.
/// Matches the PDF's electricity / water / natural-gas split.
/// </summary>
public enum UtilityType
{
    /// <summary>Electricity (elektrik).</summary>
    Electricity = 0,

    /// <summary>Water (su).</summary>
    Water = 1,

    /// <summary>Natural gas (doğalgaz).</summary>
    NaturalGas = 2,
}
