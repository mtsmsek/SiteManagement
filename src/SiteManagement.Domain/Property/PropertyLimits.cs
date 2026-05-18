namespace SiteManagement.Domain.Property;

/// <summary>
/// Numeric / length limits used across the Property bounded context. Kept
/// in one place so invariant tests, value object validators, and validators
/// in the Application layer all agree on the same numbers.
/// </summary>
public static class PropertyLimits
{
    /// <summary>Minimum allowed apartment number on a block.</summary>
    public const int ApartmentNumberMin = 1;

    /// <summary>Maximum allowed apartment number on a block.</summary>
    public const int ApartmentNumberMax = 999;

    /// <summary>Most negative floor accepted (basements).</summary>
    public const int FloorMin = -5;

    /// <summary>Highest floor accepted.</summary>
    public const int FloorMax = 50;

    /// <summary>Maximum length of a block name.</summary>
    public const int BlockNameMaxLength = 50;

    /// <summary>Maximum length of a site name.</summary>
    public const int SiteNameMaxLength = 120;

    /// <summary>Maximum length of a site address.</summary>
    public const int SiteAddressMaxLength = 250;
}
