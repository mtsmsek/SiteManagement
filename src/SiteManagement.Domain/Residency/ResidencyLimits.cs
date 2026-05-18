namespace SiteManagement.Domain.Residency;

/// <summary>
/// Numeric / length limits used across the Residency bounded context.
/// Centralised so invariant tests, value object validators, and the
/// Application-layer FluentValidation rules all share the same numbers.
/// </summary>
public static class ResidencyLimits
{
    /// <summary>Required length of a Turkish citizenship number.</summary>
    public const int TcNoLength = 11;

    /// <summary>Maximum length of a first or last name component.</summary>
    public const int NameComponentMaxLength = 60;

    /// <summary>Maximum length of a free-form email address.</summary>
    public const int EmailMaxLength = 254;

    /// <summary>Maximum length of a Turkish-format E.164 mobile/landline number.</summary>
    public const int PhoneNumberMaxLength = 16;

    /// <summary>Maximum length of a free-form vehicle description note.</summary>
    public const int VehicleNoteMaxLength = 120;

    /// <summary>Maximum stored length of a canonical TR license plate (uppercase, no separators).</summary>
    public const int PlateNumberMaxLength = 16;
}
