namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Stable resource keys for Residency-context domain exceptions. The
/// Application layer's <c>ExceptionTranslationBehavior</c> resolves each key
/// against the <c>ErrorMessages</c> resource bundle.
/// </summary>
public static class ResidencyMessageKeys
{
    /// <summary><c>"Residency.TcNo.Invalid"</c> — wrong length, non-digit, or checksum failure.</summary>
    public const string TcNoInvalid = "Residency.TcNo.Invalid";

    /// <summary><c>"Residency.FullName.Invalid"</c> — empty / whitespace / too long first or last name.</summary>
    public const string FullNameInvalid = "Residency.FullName.Invalid";

    /// <summary><c>"Residency.Email.Invalid"</c> — does not match the basic email shape or exceeds the limit.</summary>
    public const string EmailInvalid = "Residency.Email.Invalid";

    /// <summary><c>"Residency.PhoneNumber.Invalid"</c> — not a valid TR-format phone number.</summary>
    public const string PhoneNumberInvalid = "Residency.PhoneNumber.Invalid";

    /// <summary><c>"Residency.PlateNumber.Invalid"</c> — does not match the Turkish plate pattern.</summary>
    public const string PlateNumberInvalid = "Residency.PlateNumber.Invalid";

    /// <summary><c>"Residency.Vehicle.DuplicatePlate"</c> — a vehicle with the same plate is already registered.</summary>
    public const string DuplicateVehiclePlate = "Residency.Vehicle.DuplicatePlate";

    /// <summary><c>"Residency.Vehicle.NotFound"</c> — vehicle plate not present on this resident.</summary>
    public const string VehicleNotFound = "Residency.Vehicle.NotFound";

    /// <summary><c>"Residency.VehicleNote.TooLong"</c> — note longer than the allowed limit.</summary>
    public const string VehicleNoteTooLong = "Residency.VehicleNote.TooLong";
}
