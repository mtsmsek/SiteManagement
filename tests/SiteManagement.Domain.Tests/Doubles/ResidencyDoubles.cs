using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Domain.Tests.Doubles;

/// <summary>
/// Shared factory helpers for Residency-context tests. Keeps each test
/// focused on the single invariant it exercises by hiding the well-formed
/// happy-path inputs.
/// </summary>
public static class ResidencyDoubles
{
    /// <summary>Algorithm-valid sample TC number (synthetic, not a real person).</summary>
    public const string SampleTcRaw = "10000000146";

    /// <summary>Second algorithm-valid sample TC number.</summary>
    public const string SampleTcRaw2 = "12345678950";

    /// <summary>Default sample plate (Istanbul, 3 letters + 3 digits).</summary>
    public const string SamplePlateRaw = "34ABC123";

    /// <summary>Convenience: well-formed <see cref="TcNo"/>.</summary>
    public static TcNo SampleTc() => TcNo.From(SampleTcRaw);

    /// <summary>Convenience: well-formed <see cref="FullName"/>.</summary>
    public static FullName SampleFullName(string first = "Ada", string last = "Lovelace")
        => FullName.Create(first, last);

    /// <summary>Convenience: well-formed <see cref="Email"/>.</summary>
    public static Email SampleEmail(string raw = "ada@example.com") => Email.From(raw);

    /// <summary>Convenience: well-formed <see cref="PhoneNumber"/>.</summary>
    public static PhoneNumber SamplePhone(string raw = "05321234567") => PhoneNumber.From(raw);

    /// <summary>Convenience: well-formed <see cref="VehicleInfo"/>.</summary>
    public static VehicleInfo SampleVehicle(string plate = SamplePlateRaw, string? note = null)
        => VehicleInfo.Create(PlateNumber.From(plate), note);

    /// <summary>Convenience: a fully-formed <see cref="Resident"/> via the canonical factory.</summary>
    public static Resident SampleResident(
        string? tc = null,
        string firstName = "Ada",
        string lastName = "Lovelace",
        string email = "ada@example.com",
        string phone = "05321234567")
        => Resident.Create(
            TcNo.From(tc ?? SampleTcRaw),
            FullName.Create(firstName, lastName),
            Email.From(email),
            PhoneNumber.From(phone));
}
